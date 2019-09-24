#if UNITY_XR_ARKIT
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using UnityEngine;

namespace Juniper.World.LightEstimation
{
    public abstract class ARKitLightMeasurement : AbstractLightMeasurement
    {
        /// <summary>
        /// Sets up the ARKit light estimation system.
        /// </summary>
        void Start()
        {
            if (!Application.isEditor)
            {
                UnityARSessionNativeInterface.ARFrameUpdatedEvent += UnityARSessionNativeInterface_ARFrameUpdatedEvent;
                if (Find.Any(out UnityARCameraManager arkit))
                {
                    arkit.enableLightEstimation = true;
                }
                else
                {
                    Debug.LogWarning("Could not find UnityARCameraManager");
                }
            }
        }

        /// <summary>
        /// Process the ARKit light estimation values.
        /// </summary>
        private void UnityARSessionNativeInterface_ARFrameUpdatedEvent(UnityARCamera camera)
        {
            if (camera.lightData.arLightingType == LightDataType.LightEstimate)
            {
                lastIntensity = Units.Lumens.Brightness(camera.lightData.arLightEstimate.ambientIntensity);
                lastColor = camera.lightData.arLightEstimate.ambientColorTemperature.FromKelvinToColor();
            }
        }
    }
}
#endif
