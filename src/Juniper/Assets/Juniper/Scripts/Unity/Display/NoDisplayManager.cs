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
            var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
            if (!string.IsNullOrEmpty(joystick))
            {
                cameraCtrl.mode = CameraControl.Mode.Gamepad;
            }
            else if (UnityInput.mousePresent)
            {
                cameraCtrl.mode = CameraControl.Mode.Mouse;
            }
            else if (UnityInput.touchSupported)
            {
                cameraCtrl.mode = CameraControl.Mode.Touch;
            }
        }
    }
}
