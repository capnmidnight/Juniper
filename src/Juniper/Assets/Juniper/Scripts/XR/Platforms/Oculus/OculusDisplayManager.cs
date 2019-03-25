#if OCULUS
using UnityEngine;
using System.Linq;

namespace Juniper.Unity.Display
{
    public class OculusDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            var baseInstall = base.Install(reset);

            var mgr = this.EnsureComponent<OVRManager>();
            if (mgr.IsNew)
            {
                mgr.Value.useRecommendedMSAALevel = true;
                mgr.Value.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
                mgr.Value.usePositionTracking = true;
                mgr.Value.useRotationTracking = true;
                mgr.Value.useIPDInPositionTracking = true;
                mgr.Value.resetTrackerOnLoad = false;
                mgr.Value.AllowRecenter = false;
                mgr.Value.chromatic = true;
            }

            if (Application.isPlaying)
            {
                if (OVRManager.tiledMultiResSupported)
                {
                    OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHigh;
                }

                if (OVRManager.display.displayFrequenciesAvailable.Length > 0)
                {
                    OVRManager.display.displayFrequency = OVRManager.display.displayFrequenciesAvailable.Max();
                }
            }

            return baseInstall;
        }

        public override void Uninstall()
        {
            this.RemoveComponent<OVRManager>();

            base.Uninstall();
        }
    }
}
#endif
