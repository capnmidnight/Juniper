using Juniper.Unity.World.Climate;

using UnityEngine;

namespace Juniper.Unity.World
{
    /// <summary>
    /// Receives a wind speed and direction value from a wether reporting service and applies the
    /// values to a Unity Wind Zone. This component rquires a Unity WindZone component to also be on
    /// the gameObject. This component executes in edit mode.
    /// </summary>
    [ExecuteInEditMode]
#if UNITY_MODULES_WIND
    [RequireComponent(typeof(WindZone))]
#endif
    public class OutdoorWindEstimate : MonoBehaviour
    {
        /// <summary>
        /// The weather service that will retrieve the wind report.
        /// </summary>
        public Weather weatherService;

        /// <summary>
        /// When set to true, skips retrieving the wind speed and direction from the weather report
        /// and instead uses a fake value set in the Unity Editor.
        /// </summary>
        public bool FakeWind;

        /// <summary>
        /// If <see cref="FakeWind"/> is set to true, this value is the static value used for the
        /// wind speed. If it is false, this value is set to the WindSpeed value from the weather report.
        /// </summary>
        [Range(0, 300)]
        public float WindSpeed;

        /// <summary>
        /// If <see cref="FakeWind"/> is set to true, this value is the static value used for the
        /// wind direction. If it is false, this value is set to the WindDirection value from the
        /// weather report.
        /// </summary>
        [Range(0, 360)]
        public float WindDirection;

        /// <summary>
        /// The wind speed values tend to look too fast, so this value can be used to scale the speed
        /// down to something that looks better.
        /// </summary>
        [Range(0, 1)]
        public float windSpeedScale = 1;

#if UNITY_MODULES_WIND

        /// <summary>
        /// The wind zone that is being affected by the weather report.
        /// </summary>
        private WindZone wind;

#endif

        /// <summary>
        /// Returns true if the user has selected <see cref="FakeWind"/>, or if the weather service
        /// is not available, or if the magnetic compass is not available.
        /// </summary>
        /// <value><c>true</c> if use fake intensity; otherwise, <c>false</c>.</value>
        private bool UseFakeIntensity
        {
            get
            {
                return FakeWind || weatherService == null || !UnityEngine.Input.compass.enabled;
            }
        }

        /// <summary>
        /// Update the wind zone with either the static values (if <see cref="FakeWind"/> is true) or
        /// the values from the weather report (if FakeWind is false).
        /// </summary>
        public void Update()
        {
            if (weatherService == null)
            {
                weatherService = ComponentExt.FindAny<Weather>();
            }

#if UNITY_MODULES_WIND
            if (wind == null)
            {
                wind = GetComponent<WindZone>();
            }
#endif

            if (!UseFakeIntensity)
            {
                if (weatherService.WindDirection != null)
                {
                    WindDirection = weatherService.WindDirection.Value;
                }

                if (weatherService.WindSpeed != null)
                {
                    WindSpeed = weatherService.WindSpeed.Value;
                }
            }

#if UNITY_MODULES_WIND
            wind.transform.rotation = Quaternion.Euler(0, WindDirection, 0);
            wind.windMain = windSpeedScale * WindSpeed;
#endif
        }
    }
}
