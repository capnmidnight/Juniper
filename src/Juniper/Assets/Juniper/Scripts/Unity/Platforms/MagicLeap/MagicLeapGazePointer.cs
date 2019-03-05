#if MAGIC_LEAP

using Juniper.Unity.Haptics;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Unity.Input.Pointers.Gaze
{
    public abstract class MagicLeapGazePointer<ButtonIDType, HapticsType, ConfigType> :
        AbstractPointerDevice<ButtonIDType, HapticsType, ConfigType>
        where ButtonIDType : struct
        where HapticsType : AbstractHapticDevice
        where ConfigType : AbstractPointerConfiguration<ButtonIDType>, new()
    {
        private MLResult startResult;

        public override bool IsConnected
        {
            get
            {
                return MLEyes.IsStarted && startResult.IsOk;
            }
        }

        public override void Awake()
        {
            base.Awake();

            startResult = MLEyes.Start();
        }

        public void OnDestroy()
        {
            if (IsConnected)
            {
                MLEyes.Stop();
            }
        }

        private Vector3 lastFixation;

        public override Vector3 WorldPoint
        {
            get
            {
                try
                {
                    if (IsConnected)
                    {
                        var fix = MLEyes.FixationPoint;
                        if (MLEyes.LeftEye.IsBlinking || MLEyes.RightEye.IsBlinking)
                        {
                            fix = lastFixation;
                        }
                        else
                        {
                            lastFixation = fix;
                        }
                        return fix;
                    }
                }
                catch
                {
                    startResult = new MLResult(MLResultCode.UnspecifiedFailure);
                }

                return WorldFromViewport(VIEWPORT_MIDPOINT);
            }
        }

        public override Vector2 ScreenPoint
        {
            get
            {
                return ScreenFromWorld(InteractionEndPoint);
            }
        }

        public override Vector2 ViewportPoint
        {
            get
            {
                return ViewportFromWorld(InteractionEndPoint);
            }
        }
    }
}

#endif