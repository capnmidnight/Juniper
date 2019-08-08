#if UNITY_XR_OCULUS

using System.Linq;
using Juniper.Input;
using UnityEngine;

namespace Juniper.Display
{
    public class OculusDisplayManager : AbstractDisplayManager
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                var input = ComponentExt.FindAny<UnifiedInputModule>();
#if UNITY_EDITOR
                cameraCtrl.mode = CameraControl.Mode.MouseLocked;
#else
                cameraCtrl.mode = CameraControl.Mode.None;
#endif
            }

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

                if (OVRManager.display != null
                    && OVRManager.display.displayFrequenciesAvailable.Length > 0)
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