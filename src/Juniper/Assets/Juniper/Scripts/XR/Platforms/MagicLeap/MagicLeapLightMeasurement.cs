#if UNITY_XR_MAGICLEAP
using UnityEngine.XR.MagicLeap;

namespace Juniper.Unity.World.LightEstimation
{
    public abstract class MagicLeapLightMeasurement : AbstractLightMeasurement
    {
        public override void Awake()
        {
            base.Awake();

            if (!MLLightingTracker.IsStarted && !UseFakeIntensity)
            {
                var result = MLLightingTracker.Start();
                useFake = !result.IsOk;
            }
        }

        public void OnDestroy()
        {
            if (MLLightingTracker.IsStarted)
            {
                MLLightingTracker.Stop();
            }
        }

        protected override void UpdateMeasurement()
        {
            base.UpdateMeasurement();

            lastIntensity = Units.Nits.Brightness(MLLightingTracker.AverageLuminance);
            lastColor = MLLightingTracker.GlobalTemperatureColor;
        }
    }
}
#endif