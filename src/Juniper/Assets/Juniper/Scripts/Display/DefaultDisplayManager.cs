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
                if (ComponentExt.FindAny(out UnifiedInputModule input))
                {
#if UNITY_EDITOR
                    if ((input.mode & InputMode.Touch) != 0)
                    {
#if UNITY_STANDALONE || UNITY_WSA
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
#else
                        cameraCtrl.mode = CameraControl.Mode.Touch;
#endif
                    }
                    else if ((input.mode & InputMode.Mouse) != 0)
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                    }
                    else if (UnifiedInputModule.HasGamepad)
                    {
                        cameraCtrl.mode = CameraControl.Mode.Gamepad;
                    }
#elif UNITY_WSA
                    if ((input.mode & InputMode.Touch) != 0
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    {
                        cameraCtrl.mode = CameraControl.Mode.Touch;
                    }
                    else if ((input.mode & InputMode.Mouse) != 0
                        && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                    }

#else
                    if ((input.mode & InputMode.Touch) != 0
                        && Application.isMobilePlatform)
                    {
                        cameraCtrl.mode = CameraControl.Mode.Touch;
                    }
                    else if ((input.mode & InputMode.Mouse) != 0
                        && UnityInput.mousePresent)
                    {
                        cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                    }
#endif
                    else if ((input.mode & InputMode.Mouse) != 0)
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
