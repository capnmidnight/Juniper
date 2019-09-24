#if UNITY_XR_ARKIT
using UnityEngine;

namespace Juniper.Ground
{
    public class ARKitGround : AbstractGround
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

        protected override void Awake()
        {
            base.Awake();

            if(Find.Any(out UnityARCameraManager camMgr))
            {
                camMgr.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
            }
            generatePlanes = this.Ensure<UnityARGeneratePlane>();
            generatePlanes.planePrefab = planePrefab;
        }
    }
}
#endif
