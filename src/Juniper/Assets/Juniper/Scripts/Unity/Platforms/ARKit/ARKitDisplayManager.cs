#if UNITY_XR_ARKIT
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Juniper.Unity.Display
{
    public class ARKitDisplayManager : AbstractPassthroughDisplayManager
    {
#if !UNITY_EDITOR
        public override void Install(bool reset)
        {
            reset &= Application.isEditor;

            base.Install(reset);

            this.WithLock(() =>
            {
                var bgRenderer = this.EnsureComponent<UnityARVideo>();
                bgRenderer.m_ClearMaterial = ARBackgroundMaterial;

                this.EnsureComponent<UnityARCameraNearFar>();

                var camMgr = this.EnsureComponent<UnityARCameraManager>();
                camMgr.startAlignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
                camMgr.planeDetection = UnityARPlaneDetection.None;
                camMgr.getPointCloud = enablePointCloud;
                camMgr.enableAutoFocus = true;
            });
        }

        public override void Uninstall()
        {
            this.RemoveComponent<UnityARCameraManager>();
            this.RemoveComponent<UnityARCameraNearFar>();
            this.RemoveComponent<UnityARVideo>();

            base.Uninstall();
        }
#endif
    }
}
#endif
