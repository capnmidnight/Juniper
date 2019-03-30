#if UNITY_XR_ARCORE
using GoogleARCore;
using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
{
    public abstract class ARCoreLightMeasurement : AbstractLightMeasurement
    {
        /// <summary>
        /// Sets up the ARCore light estimation system.
        /// </summary>
        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                var arCoreSession = ComponentExt.FindAny<ARCoreSession>();
                if(arCoreSession?.SessionConfig == null)
                {
                    return false;
                }

                arCoreSession.SessionConfig.EnableLightEstimation = true;
                return true;
            }
            return false;
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