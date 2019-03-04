#if ARKIT
using UnityEngine;

namespace Juniper.Ground
{
    public class ARKitGround : AbstractARGround
    {
        /// <summary>
        /// The clonable object to use as a ground plane in ARKit systems.
        /// </summary>
        [Header("Apple ARKit")]
        public GameObject planePrefab;

        /// <summary>
        /// When running on ARKit, this is the component that generates ground planes.
        /// </summary>
        UnityARGeneratePlane generatePlanes;

        protected override void InternalStart(XRSystem xr)
        {
            var camMgr = ComponentExt.FindAny<UnityARCameraManager>();
            camMgr.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
            generatePlanes = this.EnsureComponent<UnityARGeneratePlane>();
            generatePlanes.planePrefab = planePrefab;
        }
    }
}
#endif