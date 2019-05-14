#if UNITY_XR_ARCORE
using GoogleARCore;
using UnityEngine;

namespace Juniper.World.LightEstimation
{
    public abstract class ARCoreLightMeasurement : AbstractLightMeasurement
    {
        /// <summary>
        /// Sets up the ARCore light estimation system.
        /// </summary>
        public override void Install(bool reset)
        {
            base.Install(reset);

            var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
            if(arCoreSession?.SessionConfig == null)
            {
                return false;
            }

            arCoreSession.SessionConfig.EnableLightEstimation = true;
        }

        protected override void UpdateMeasurement()
        {
            base.UpdateMeasurement();

            if (Frame.LightEstimate.State == LightEstimateState.Valid)
            {
                lastIntensity = Frame.LightEstimate.PixelIntensity;
                lastColor = Frame.LightEstimate.ColorCorrection;
            }
        }
    }
}
#endif
