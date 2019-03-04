using Juniper.World.GIS;

using System;

namespace Juniper.World.Climate
{
    public interface IWeatherReport
    {
        /// <summary>
        /// timestamp for the period of time the report covers.
        /// </summary>
        DateTime ReportTime
        {
            get;
        }

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        string City
        {
            get;
        }

        /// <summary>
        /// The country for which the report covers.
        /// </summary>
        string Country
        {
            get;
        }

        /// <summary>
        /// The latitude/longitude for which the report covers.
        /// </summary>
        LatLngPoint Location
        {
            get;
        }

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        float? Visibility
        {
            get;
        }

        /// <summary>
        /// The time at which sunrise occurs for the <see cref="ReportTime"/>
        /// </summary>
        /// <value>The sunrise time.</value>
        DateTime? SunriseTime
        {
            get;
        }

        /// <summary>
        /// The time at which sunset occurs for the <see cref="ReportTime"/>
        /// </summary>
        /// <value>The sunset time.</value>
        DateTime? SunsetTime
        {
            get;
        }

        /// <summary>
        /// An estimate, from 0 to 1, of the current proportion cloud cover.
        /// </summary>
        float? CloudCover
        {
            get;
        }

        /// <summary>
        /// The current direction of the wind, in degrees clockwise from north.
        /// </summary>
        float? WindDirection
        {
            get;
        }

        /// <summary>
        /// The current speed of the wind, in KMH.
        /// </summary>
        float? WindSpeed
        {
            get;
        }

        /// <summary>
        /// An enumeration of the current conditions.
        /// </summary>
        /// <value>The conditions.</value>
        string Conditions
        {
            get;
        }

        /// <summary>
        /// The current temperature.
        /// </summary>
        float? Temperature
        {
            get;
        }

        /// <summary>
        /// The air humidity.
        /// </summary>
        float? Humidity
        {
            get;
        }

        /// <summary>
        /// The atmospheric pressure.
        /// </summary>
        float? AtmosphericPressure
        {
            get;
        }

        /// <summary>
        /// The error message, if a server error occurred.
        /// </summary>
        /// <value>The error.</value>
        string Error
        {
            get;
        }
    }
}