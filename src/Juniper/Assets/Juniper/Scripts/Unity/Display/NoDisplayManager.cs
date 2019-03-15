using Juniper.Unity.Input;

using System.Linq;
using UnityEngine;
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

                if (Application.isMobilePlatform)
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if (UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.Mouse;
                }
                else if (!string.IsNullOrEmpty(joystick))
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
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
