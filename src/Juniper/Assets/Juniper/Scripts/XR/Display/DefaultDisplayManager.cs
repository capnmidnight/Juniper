using System;
using System.Linq;

using Juniper.Unity.Input;

using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Display
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

        public override void Start()
        {
            base.Start();

            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                var input = ComponentExt.FindAny<UnifiedInputModule>();
                var hasJoystick = UnityInput.GetJoystickNames()
                    .Any(j => !string.IsNullOrEmpty(j));
#if UNITY_EDITOR
                if (input.mode.HasFlag(UnifiedInputModule.Mode.Touch))
                {
#if UNITY_STANDALONE || UNITY_WSA
                    cameraCtrl.mode = CameraControl.Mode.MouseLocked;
#else
                    cameraCtrl.mode = CameraControl.Mode.Touch;
#endif
                }
                else if (input.mode.HasFlag(UnifiedInputModule.Mode.Mouse))
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                }
                else if (hasJoystick)
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
#elif UNITY_WSA
                if (input.mode.HasFlag(UnifiedInputModule.Mode.Touch)
                    && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if (input.mode.HasFlag(UnifiedInputModule.Mode.Mouse)
                    && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                }

#else
                if (input.mode.HasFlag(UnifiedInputModule.Mode.Touch)
                    && Application.isMobilePlatform)
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if (input.mode.HasFlag(UnifiedInputModule.Mode.Mouse)
                    && UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                }
#endif
                else if (input.mode.HasFlag(UnifiedInputModule.Mode.Mouse))
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                }
                else
                {
                    cameraCtrl.mode = CameraControl.Mode.None;
                }
                ScreenDebugger.Print($"Mode is {cameraCtrl.mode}");
            }
            else if (cameraCtrl.mode == CameraControl.Mode.MagicWindow)
            {
                UnityInput.gyro.enabled = true;
                UnityInput.compensateSensors = true;
            }
        }
    }
}
