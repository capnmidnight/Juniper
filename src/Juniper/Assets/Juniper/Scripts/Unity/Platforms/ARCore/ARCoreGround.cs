#if UNITY_XR_ARCORE
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

namespace Juniper.Unity.Ground
{
    public class ARCoreGround : AbstractARGround
    {
        /// <summary>
        /// When running on ARCore, a collection of all the planes that ARCore is tracking.
        /// </summary>
        List<DetectedPlane> newPlanes;

        protected override void InternalStart(JuniperPlatform xr)
        {
            var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
            arCoreSession.SessionConfig.PlaneFindingMode = DetectedPlaneFindingMode.HorizontalAndVertical;
            newPlanes = new List<DetectedPlane>();
        }

        public override void Update()
        {
            bool isTracking = Session.Status == SessionStatus.Tracking;
            if (isTracking)
            {
                Session.GetTrackables(newPlanes, TrackableQueryFilter.New);
                for (int i = 0; i < newPlanes.Count; i++)
                {
                    var planeVisualizer = ARCoreGroundPlaneVisualizer.Initialize(newPlanes[i]);
                    planeVisualizer.CurrentGroundMeshMaterial = CurrentMaterial;
                    planeVisualizer.gameObject.layer = GroundLayer;
                    planeVisualizer.transform.SetParent(transform, true);
                }
            }

            base.Update();
        }
    }
}
#endif
