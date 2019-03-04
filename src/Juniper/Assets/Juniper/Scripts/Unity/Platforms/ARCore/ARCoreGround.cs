#if ARCORE
using System.Collections.Generic;

namespace Juniper.Ground
{
    public class ARCoreGround : AbstractARGround
    {
        /// <summary>
        /// When running on ARCore, a collection of all the planes that ARCore is tracking.
        /// </summary>
        List<TrackedPlane> newPlanes;

        protected override void InternalStart(XRSystem xr)
        {
            var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
            arCoreSession.SessionConfig.EnablePlaneFinding = true;
            newPlanes = new List<TrackedPlane>();
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
