#if GOOGLEVR
using Juniper.Input;
using System.Linq;
using UnityEngine;

namespace Juniper.Display
{
    public class DaydreamDisplayManager : AbstractDisplayManager
    {
        public static bool AnyActiveGoogleInstantPreview
        {
            get
            {
                return ComponentExt.FindAll<Gvr.Internal.InstantPreview>()
                    .Any(ComponentExt.IsActivated);
            }
        }

        public override void Install(bool reset)
        {
            base.Install(reset);

            this.Ensure<GvrHeadset>();
        }

        public override void Start()
        {
            base.Start();

            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                cameraCtrl.mode = CameraControl.Mode.None;
            }
        }

        public override void Uninstall()
        {
            this.Remove<GvrHeadset>();

            base.Uninstall();
        }
    }
}
#endif
