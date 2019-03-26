#if UNITY_XR_OCULUS
using UnityEngine;
using System.Linq;

namespace Juniper.Unity.Display
{
    public class OculusDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            if(base.Install(reset))
            {
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

                return true;
            }

            return false;
        }

        public override void Uninstall()
        {
            this.Remove<OVRManager>();

            base.Uninstall();
        }
    }
}
#endif
