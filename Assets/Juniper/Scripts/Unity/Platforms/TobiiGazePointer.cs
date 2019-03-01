#if TOBII

using Juniper.Haptics;
using Juniper.Input.Pointers.Screen;
using Tobii.Gaming;

using UnityEngine;

namespace Juniper.Input.Pointers.Gaze
{
    public abstract class TobiiGazePointer<ButtonIDType, HapticsType, ConfigType> :
        AbstractScreenDevice<ButtonIDType, HapticsType, ConfigType>
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        public override bool IsConnected =>
            TobiiAPI.IsConnected && TobiiAPI.GetUserPresence().IsUserPresent();

        private GazePoint lastGaze;

        public override Vector2 ScreenPoint
        {
            get
            {
                if (IsConnected)
                {
                    var validGazeCount = 0;

                    foreach (var gaze in TobiiAPI.GetGazePointsSince(lastGaze))
                    {
                        if (gaze.IsValid)
                        {
                            if (validGazeCount > 0)
                            {
                                var point = (Vector3)lastGaze.Screen + pointerOffset;
                                motionFilter?.UpdateState(EventCamera.ScreenToWorldPoint(point));
                            }
                            lastGaze = gaze;
                            ++validGazeCount;
                        }
                    }
                }

                if (lastGaze.IsValid)
                {
                    return lastGaze.Screen;
                }
                else
                {
                    return SCREEN_MIDPOINT;
                }
            }
        }

        private HeadPose lastPose = HeadPose.Invalid;

        public override Vector3 CameraPositionOffset
        {
            get
            {
                foreach (var pose in TobiiAPI.GetHeadPosesSince(lastPose))
                {
                    if (pose.IsValid)
                    {
                        lastPose = pose;
                    }
                }

                if (lastPose.IsValid)
                {
                    return lastPose.Position;
                }
                else
                {
                    return base.CameraPositionOffset;
                }
            }
        }
    }
}

#endif
