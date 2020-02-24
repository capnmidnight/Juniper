using System;

using Juniper.Input;

using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Juniper.Display
{
    public abstract class DefaultDisplayManager : AbstractDisplayManager
    {
        protected override float DEFAULT_FOV
        {
            get
            {
                return 50;
            }
        }

        public override void Update()
        {
            if (cameraCtrl.ControlMode == CameraControl.Mode.Auto)
            {
                if (Find.Any(out UnifiedInputModule input))
                {
#if UNITY_WSA
                    if (input.TouchRequested
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    {
                        cameraCtrl.playerMode = CameraControl.Mode.Touch;
                    }
                    else if (input.MouseRequested
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    {
                        cameraCtrl.playerMode = CameraControl.Mode.MouseLocked;
                    }

#else
                    if (input.TouchRequested
                        && Application.isMobilePlatform)
                    {
                        cameraCtrl.playerMode = CameraControl.Mode.Touch;
                    }
                    else if (input.MouseRequested
                        && UnityInput.mousePresent)
                    {
                        cameraCtrl.playerMode = CameraControl.Mode.MouseLocked;
                    }
#endif
                    else
                    {
                        cameraCtrl.playerMode = CameraControl.Mode.None;
                    }
                    ScreenDebugger.Print($"Mode is {cameraCtrl.playerMode.ToString()}");
                }
            }
            else if (cameraCtrl.ControlMode == CameraControl.Mode.MagicWindow)
            {
                UnityInput.gyro.enabled = true;
                UnityInput.compensateSensors = true;
            }

            base.Update();
        }
    }
}
