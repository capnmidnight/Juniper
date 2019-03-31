using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
{
    /// <summary>
    /// Abstracts the measurement of light intensity from camera frames for Augmented Reality Light
    /// Estimation. This component executes in edit mode.
    /// </summary>
    [ExecuteInEditMode]
    public class LightMeasurement :
#if UNITY_XR_ARKIT
        ARKitLightMeasurement
#elif UNITY_XR_ARCORE
        ARCoreLightMeasurement
#elif UNITY_XR_MAGICLEAP
        MagicLeapLightMeasurement
#elif VUFORIA
        VuforiaLightMeasurement
#else
        AbstractLightMeasurement
#endif
    {
    }
}
