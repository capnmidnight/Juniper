using System;

using Juniper.Unity.World.Climate;
using Juniper.Unity.World.GIS;

using UnityEngine;

namespace Juniper.Unity.World.LightEstimation
{
    /// <summary>
    /// Modifies the base light estimate to include cloud cover values retrieved from a weather
    /// reporting service, and sun position values calculated from GPS and time. Only one of these
    /// components may be included on a gameObject. This component requires a Unity Light component.
    /// This component runs in the editor during edit mode.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class OutdoorLightEstimate : AbstractLightEstimate
    {
        /// <summary>
        /// The weather service used to retrieve the cloud cover, sunrise, and sunset values. Cloud
        /// cover is used to soften cast shadows. Sunrise and sunset are used to determine night time.
        /// </summary>
        public Weather weather;

        /// <summary>
        /// Gets the current weather report for cloud cover, unless it's night time, using an indoor
        /// lighting model instead.
        /// </summary>
        /// <value>The cloud cover.</value>
        protected override float CloudCover
        {
            get
            {
                if (HasCloudCover)
                {
                    return weather.CloudCover.Value;
                }
                else
                {
                    return 0.25f;
                }
            }
        }

        /// <summary>
        /// Returns true if it is not night time, we have a <see cref="sunPosition"/> component, and
        /// that component is capable of calculating the sun position.
        /// </summary>
        /// <value><c>true</c> if has sun rotation; otherwise, <c>false</c>.</value>
        protected override bool HasSunRotation
        {
            get
            {
                return !IsNight
                    && sunPosition != null
                    && sunPosition.CanCalculateCurrentLocalPosition;
            }
        }

        /// <summary>
        /// Gets a rotation quaternion that will put the "sun dot" of Unity's default skybox into
        /// the correct position in the sky, unless it's night time, using an indoor lighting model instead.
        /// </summary>
        /// <value>The sun rotation.</value>
        protected override Vector3 SunRotation
        {
            get
            {
                if (HasSunRotation)
                {
                    return sunPosition.orientation.ToEuler();
                }
                else
                {
                    return overhead;
                }
            }
        }

        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                sunPosition = this.Ensure<SunPosition>();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Prints the debug report, if asked.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (weather == null)
            {
                weather = ComponentExt.FindAny<Weather>();
            }

            if (DebugReport)
            {
                PrintDebugReport();
            }
        }

        /// <summary>
        /// Is set to true when the current time is outside of the [sunRise, sunSet] range, or if the
        /// sun position has fallen below the horizon
        /// </summary>
        private readonly bool isNight;

        /// <summary>
        /// The component used for calculating the position of the sun.
        /// </summary>
        private SunPosition sunPosition;

        /// <summary>
        /// Returns true if the current time is past the sunset time, or if the sunset time is not
        /// know, the sun position is below the horizon.
        /// </summary>
        /// <value><c>true</c> if is night; otherwise, <c>false</c>.</value>
        private bool IsNight
        {
            get
            {
                var weatherHasSunset = weather != null && weather.Sunset != null;
                var timeIsAfterSunset = weatherHasSunset && EditableDateTime.Now > weather.Sunset.Value;
                var sunPositionCanCalculate = sunPosition != null && sunPosition.CanCalculateCurrentLocalPosition;
                var sunIsBelowHorizon = sunPositionCanCalculate && sunPosition.orientation.AltitudeDegrees < 0;

                return (weatherHasSunset && timeIsAfterSunset)
                    || (!weatherHasSunset && sunPositionCanCalculate && sunIsBelowHorizon);
            }
        }

        /// <summary>
        /// Returns true if it is not night time, we have a <see cref="weather"/> component, and that
        /// component has a cloud cover value.
        /// </summary>
        /// <value><c>true</c> if has cloud cover; otherwise, <c>false</c>.</value>
        private bool HasCloudCover
        {
            get
            {
                return !IsNight
                    && weather != null
                    && weather.CloudCover != null;
            }
        }

        /// <summary>
        /// If <see cref="IndoorLightEstimate.DebugReport"/> is true, prints a report that displays
        /// the status of the component.
        /// </summary>
        private void PrintDebugReport()
        {
            if (weather == null)
            {
                ScreenDebugger.Print("  no weather service");
            }

            if (sunPosition == null)
            {
                ScreenDebugger.Print("  no sun position");
            }
        }
    }
}
