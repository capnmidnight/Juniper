using Juniper.Unity.Input;

using System.Linq;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Display
{
    public abstract class NoDisplayManager : AbstractDisplayManager
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
                var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
                
#if UNITY_STANDALONE || UNITY_WSA
                if (!string.IsNullOrEmpty(joystick))
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
                else if (UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.Mouse;
                }
                else
#endif
                if (UnityInput.touchSupported)
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
#if !(UNITY_STANDALONE || UNITY_WSA)
                else if (!string.IsNullOrEmpty(joystick))
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
                else if (UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.Mouse;
                }
#endif
            }
            else if (cameraCtrl.mode == CameraControl.Mode.MagicWindow)
            {
                UnityInput.gyro.enabled = true;
                UnityInput.compensateSensors = true;
            }
        }
    }
}
