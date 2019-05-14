#if UNITY_XR_OCULUS

using System.Linq;

using UnityEngine;

namespace Juniper.Display
{
    public class OculusDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            var mgr = this.Ensure<OVRManager>();
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
        }

        public override void Uninstall()
        {
            this.Remove<OVRManager>();

            base.Uninstall();
        }
    }
}

#endif