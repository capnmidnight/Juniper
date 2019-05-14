#if UNITY_XR_ARCORE
using GoogleARCore;
using Juniper.Display;
using System.Collections.Generic;
using UnityEngine;

namespace Juniper.Ground
{
    public class ARCoreGround : AbstractGround
    {
        /// <summary>
        /// When running on ARCore, a collection of all the planes that ARCore is tracking.
        /// </summary>
        private readonly List<DetectedPlane> newPlanes = new List<DetectedPlane>(10);
        private DisplayManager display;

        protected override void Awake()
        {
            base.Awake();

            display = ComponentExt.FindAny<DisplayManager>();
            display.ARModeChange += Display_ARModeChange;
        }

        private void Display_ARModeChange(object sender, AugmentedRealityTypes e)
        {
            if (e == AugmentedRealityTypes.PassthroughCamera)
            {
                var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
                arCoreSession.SessionConfig.PlaneFindingMode = DetectedPlaneFindingMode.HorizontalAndVertical;
            }
        }

        public override void Update()
        {
            if (display.ARMode == AugmentedRealityTypes.PassthroughCamera && Session.Status == SessionStatus.Tracking)
            {
                Session.GetTrackables(newPlanes, TrackableQueryFilter.New);
                for (var i = 0; i < newPlanes.Count; i++)
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
