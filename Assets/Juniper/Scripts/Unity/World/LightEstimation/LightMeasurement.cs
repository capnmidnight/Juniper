using UnityEngine;

namespace Juniper.World.LightEstimation
{
    /// <summary>
    /// Abstracts the measurement of light intensity from camera frames for Augmented Reality Light
    /// Estimation. This component executes in edit mode.
    /// </summary>
    [ExecuteInEditMode]
    public class LightMeasurement :
        #if ARKIT
        ARKitLightMeasurement
#elif ARCORE
        ARCoreLightMeasurement
#elif MAGIC_LEAP
        MagicLeapLightMeasurement
#elif VUFORIA
        VuforiaLightMeasurement
#else
        NoLightMeasurement
#endif
    {
    }
}
