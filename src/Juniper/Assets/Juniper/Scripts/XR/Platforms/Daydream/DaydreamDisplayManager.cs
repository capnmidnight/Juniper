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
            var baseInstall = base.Install(reset);

            this.EnsureComponent<GvrHeadset>();

            return baseInstall;
        }

        public override void Start()
        {
            base.Start();

            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                mode = Mode.None;
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
