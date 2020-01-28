using System;

using Juniper.Climate;
using Juniper.World.GIS;

using UnityEngine;

namespace Juniper.World.Climate
{
    /// <summary>
    /// An API-compatible, editable object for making static weather reports.
    /// </summary>
    [Serializable]
    public class FakeWeatherReport : IWeatherReport
    {
        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        public EditableDateTime reportTime;

        /// <summary>
        /// The latitude/longitude for which the report covers.
        /// </summary>
        public LatLngPoint location;

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        [Range(0, 100)]
        public float visibility;

        /// <summary>
        /// An estimate, from 0 to 1, of the current proportion cloud cover.
        /// </summary>
        [Range(0, 1)]
        public float cloudCover;

        /// <summary>
        /// The current direction of the wind, in degrees clockwise from north.
        /// </summary>
        [Range(0, 360)]
        public float windDirection;

        /// <summary>
        /// The current speed of the wind, in KMH.
        /// </summary>
        [Range(0, 100)]
        public float windSpeed;

        /// <summary>
        /// The current temperature.
        /// </summary>
        [Range(-50, 50)]
        public float temperature;

        /// <summary>
        /// The air humidity.
        /// </summary>
        [Range(0, 1)]
        public float humidity;

        /// <summary>
        /// The atmospheric pressure.
        /// </summary>
        [Range(100, 100000)]
        public float atmosphericPressure;

        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        public DateTime ReportTime => reportTime;

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string City => "Fakesburg";

        /// <summary>
        /// The country for which the report covers.
        /// </summary>
        public string Country => "US";

        /// <summary>
        /// The latitude/longitude for which the report covers.
        /// </summary>
        public LatLngPoint Location => location;

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        public float? AtmosphericVisibility => visibility;

        /// <summary>
        /// The time at which sunrise occurs for the <see cref="ReportTime"/>
        /// </summary>
        /// <value>The sunrise time.</value>
        public DateTime? SunriseTime => new DateTime(reportTime.Year, (int)reportTime.Month, reportTime.Day, 6, 30, 0);

        /// <summary>
        /// The time at which sunset occurs for the <see cref="ReportTime"/>
        /// </summary>
        /// <value>The sunset time.</value>
        public DateTime? SunsetTime => new DateTime(reportTime.Year, (int)reportTime.Month, reportTime.Day, 18, 30, 0);

        /// <summary>
        /// An estimate, from 0 to 1, of the current proportion cloud cover.
        /// </summary>
        public float? CloudCover => cloudCover;

        /// <summary>
        /// The current direction of the wind, in degrees clockwise from north.
        /// </summary>
        public float? WindDirection => windDirection;

        /// <summary>
        /// The current speed of the wind, in KMH.
        /// </summary>
        public float? WindSpeed => windSpeed;

        /// <summary>
        /// An enumeration of the current conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public string Conditions => "fake weather report";

        /// <summary>
        /// The current temperature.
        /// </summary>
        public float? Temperature => temperature;

        /// <summary>
        /// The air humidity.
        /// </summary>
        public float? Humidity => humidity;

        /// <summary>
        /// The atmospheric pressure.
        /// </summary>
        public float? AtmosphericPressure => atmosphericPressure;

        /// <summary>
        /// The error message, if a server error occurred.
        /// </summary>
        /// <value>The error.</value>
        public string ErrorMessage => null;
    }
}
