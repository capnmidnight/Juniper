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

        public void Start()
        {
            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                if (Find.Any(out UnifiedInputModule input))
                {
#if UNITY_EDITOR
                    if (input.TouchRequested)
                    {
#if UNITY_STANDALONE || UNITY_WSA
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
#else
                        cameraCtrl.mode = CameraControl.Mode.Touch;
#endif
                    }
                    else if (input.MouseRequested)
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                    }
                    else if (UnifiedInputModule.HasGamepad)
                    {
                        cameraCtrl.mode = CameraControl.Mode.Gamepad;
                    }
#elif UNITY_WSA
                    if (input.TouchRequested
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    {
                        cameraCtrl.mode = CameraControl.Mode.Touch;
                    }
                    else if (input.MouseRequested
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                    }

#else
                    if (input.TouchRequested
                        && Application.isMobilePlatform)
                    {
                        cameraCtrl.mode = CameraControl.Mode.Touch;
                    }
                    else if (input.MouseRequested
                        && UnityInput.mousePresent)
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                    }
#endif
                    else if (input.MouseRequested)
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                    }
                    else
                    {
                        cameraCtrl.mode = CameraControl.Mode.None;
                    }
                    ScreenDebugger.Print($"Mode is {cameraCtrl.mode.ToString()}");
                }
            }
            else if (cameraCtrl.mode == CameraControl.Mode.MagicWindow)
            {
                UnityInput.gyro.enabled = true;
                UnityInput.compensateSensors = true;
            }
        }
    }
}
