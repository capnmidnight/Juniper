#if ARCORE
using GoogleARCore;
using UnityEngine;

namespace Juniper.World.LightEstimation
{
    public abstract class ARCoreLightMeasurement : AbstractLightMeasurement
    {
        /// <summary>
        /// Sets up the ARCore light estimation system.
        /// </summary>
        public void Start()
        {
            var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
            arCoreSession.SessionConfig.EnableLightEstimation = true;
        }

        protected override void InternalUpdate()
        {
            base.InternalUpdate();

            if (Frame.LightEstimate.State == LightEstimateState.Valid)
            {
                lastIntensity = Frame.LightEstimate.PixelIntensity;
                lastColor = Frame.LightEstimate.ColorCorrection;
            }
        }
    }
}
#endif