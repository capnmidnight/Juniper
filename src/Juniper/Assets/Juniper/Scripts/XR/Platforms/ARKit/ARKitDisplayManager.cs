#if UNITY_XR_ARKIT
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Juniper.Unity.Display
{
    public class ARKitDisplayManager : AbstractPassthroughDisplayManager
    {
#if !UNITY_EDITOR
        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                this.WithLock(() =>
                {
                    var bgRenderer = this.Ensure<UnityARVideo>();
                    bgRenderer.m_ClearMaterial = ARBackgroundMaterial;

                    this.Ensure<UnityARCameraNearFar>();

                    var camMgr = this.Ensure<UnityARCameraManager>();
                    camMgr.startAlignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
                    camMgr.planeDetection = UnityARPlaneDetection.None;
                    camMgr.getPointCloud = enablePointCloud;
                    camMgr.enableAutoFocus = true;
                });

                return true;
            }

            return false;
        }

        public override void Uninstall()
        {
            this.Remove<UnityARCameraManager>();
            this.Remove<UnityARCameraNearFar>();
            this.Remove<UnityARVideo>();

            base.Uninstall();
        }
#endif
    }
}
#endif
