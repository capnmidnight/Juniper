#if TOBII

using Juniper.Unity.Haptics;
using Juniper.Unity.Input.Pointers.Screen;
using Tobii.Gaming;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Gaze
{
    public abstract class TobiiGazePointer :
        AbstractGazePointer
    {
        public override bool IsConnected
        {
            get
            {
                return Application.isPlaying
                    && TobiiAPI.IsConnected
                    && TobiiAPI.GetUserPresence().IsUserPresent();
            }
        }

        private GazePoint lastGaze;

        public override Vector3 WorldPoint
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
                                motionFilter?.UpdateState(WorldFromScreen(point));
                            }
                            lastGaze = gaze;
                            ++validGazeCount;
                        }
                    }
                }

                if (lastGaze.IsValid)
                {
                    return WorldFromScreen(lastGaze.Screen);
                }
                else
                {
                    return WorldFromViewport(VIEWPORT_MIDPOINT);
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