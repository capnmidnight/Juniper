using Juniper.Unity.Input;

using UnityEngine;

using UnityInput = UnityEngine.Input;

#if !UNITY_EDITOR
using System.Linq;
#endif

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
#if UNITY_EDITOR
                var input = ComponentExt.FindAny<UnifiedInputModule>();
                if (input.mode.HasFlag(UnifiedInputModule.Mode.Touch))
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if(input.mode.HasFlag(UnifiedInputModule.Mode.Mouse))
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseScreenEdge;
                }
#else
                var hasJoystick = UnityInput.GetJoystickNames()
                    .Any(j => !string.IsNullOrEmpty(j));
                if (Application.isMobilePlatform)
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if (UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.MouseLocked;
                }
                else if (hasJoystick)
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
#endif
                else
                {
                    cameraCtrl.mode = CameraControl.Mode.None;
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
