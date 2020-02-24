#if UNITY_XR_GOOGLEVR_ANDROID
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
                return Find.All<Gvr.Internal.InstantPreview>()
                    .Any(ComponentExt.IsActivated);
            }
        }

        public override void Install(bool reset)
        {
            base.Install(reset);

            this.Ensure<GvrHeadset>();
        }

        public override void Uninstall()
        {
            this.Remove<GvrHeadset>();

            base.Uninstall();
        }
    }
}
#endif
