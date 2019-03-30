#if GOOGLEVR
using Juniper.Unity.Input;
using System.Linq;
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
            if(base.Install(reset))
            {
                this.Ensure<GvrHeadset>();
                return true;
            }

            return false;
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