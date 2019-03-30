#if UNITY_XR_ARKIT
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
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
                var arkit = ComponentExt.FindAny<UnityARCameraManager>();
                if (arkit == null)
                {
                    Debug.LogWarning("Could not find UnityARCameraManager");
                }
                else
                {
                    arkit.enableLightEstimation = true;
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