#if GOOGLEVR
using UnityEngine;

namespace Juniper.Unity.Display
{
    public class DaydreamDisplayManager : AbstractDisplayManager
    {
        public static bool AnyActiveGoogleInstantPreview =>
            ComponentExt.FindAll<Gvr.Internal.InstantPreview>()
                .Any(ComponentExt.IsActivated);

        public override bool Install(bool reset)
        {
            reset &= Application.isEditor;

            var baseInstall = base.Install(reset);

            this.EnsureComponent<GvrHeadset>();

            return baseInstall;
        }

        public override void Start()
        {
            base.Start();

            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                if (setMouseLock && Application.isEditor && AnyActiveGoogleInstantPreview)
                {
                    setMouseLock = false;
                }

                var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
                if (AnyActiveGoogleInstantPreview)
                {
                    mode = Mode.None;
                }
                else if (UnityInput.mousePresent)
                {
                    mode = Mode.Mouse;
                }
                else if (!string.IsNullOrEmpty(joystick))
                {
                    mode = Mode.Gamepad;
                }
            }
        }

        public override void Uninstall()
        {
            this.RemoveComponent<GvrHeadset>();

            base.Uninstall();
        }
    }
}
#endif
