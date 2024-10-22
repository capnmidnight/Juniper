using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.Climate;
using Juniper.World.GIS;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// response from the OpenWeatherMap API. <seealso cref="http://openweathermap.org/current#current_JSON"/>.
    /// </summary>
    [Serializable]
    public class WeatherReport : IWeatherReport, ISerializable
    {
        /// <summary>
        /// When not null, the error response from the server.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// response ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        public long DT { get; set; }

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        public int Visibility { get; set; }

        /// <summary>
        /// latitude/longitude for which the report covers.
        /// </summary>
        public OWMCoord Coord { get; set; }

        /// <summary>
        /// various weather conditions that the report shows.
        /// </summary>
        public IReadOnlyList<OWMWeather> Weather { get; set; }

        /// <summary>
        /// The main metrics portion of the weather forecast.
        /// </summary>
        public OWMMain Main { get; set; }

        /// <summary>
        /// The current wind speed and direction.
        /// </summary>
        public OWMWind Wind { get; set; }

        /// <summary>
        /// The current cloud cover.
        /// </summary>
        public OWMClouds Clouds { get; set; }

        /// <summary>
        /// The expected rainfall for the next three hours.
        /// </summary>
        public OWMRain Rain { get; set; }

        /// <summary>
        /// The expected snowfall for the next three hours.
        /// </summary>
        public OWMSnow Snow { get; set; }

        /// <summary>
        /// Additional geographic information about the weather report.
        /// </summary>
        public OWMSys Sys { get; set; }

        /// <summary>
        /// Deserializes a WeatherReport.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected WeatherReport(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Error = info.GetString("error");
            ID = info.GetInt32("id");
            Name = info.GetString("name");
            DT = info.GetInt64("dt");
            Visibility = info.GetInt32("visibility");
            Coord = (OWMCoord)info.GetValue("coord", typeof(OWMCoord));
            Weather = (OWMWeather[])info.GetValue("weather", typeof(OWMWeather[]));
            Main = (OWMMain)info.GetValue("main", typeof(OWMMain));
            Wind = (OWMWind)info.GetValue("wind", typeof(OWMWind));
            Clouds = (OWMClouds)info.GetValue("clouds", typeof(OWMClouds));
            Rain = (OWMRain)info.GetValue("rain", typeof(OWMRain));
            Snow = (OWMSnow)info.GetValue("snow", typeof(OWMSnow));
            Sys = (OWMSys)info.GetValue("sys", typeof(OWMSys));
        }

        /// <summary>
        /// Serializes a WeatherReport.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("error", Error);
            info.AddValue("id", ID);
            info.AddValue("name", Name);
            info.AddValue("dt", DT);
            info.AddValue("visibility", Visibility);
            info.AddValue("coord", Coord);
            info.AddValue("weather", (OWMWeather[])Weather);
            info.AddValue("main", Main);
            info.AddValue("wind", Wind);
            info.AddValue("clouds", Clouds);
            info.AddValue("rain", Rain);
            info.AddValue("snow", Snow);
            info.AddValue("sys", Sys);
        }

        /// <summary>
        /// When not null, the error response from the server.
        /// </summary>
        public string ErrorMessage => Error;

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string City => Name;

        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        private DateTime? repTime;

        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        public DateTime ReportTime
        {
            get
            {
                if (repTime is null)
                {
                    if (Error is null)
                    {
                        repTime = DT.UnixTimestampToDateTime();
                    }
                    else
                    {
                        repTime = DateTime.Now.AddMinutes(-14.5);
                    }
                }

                return repTime.Value;
            }
        }

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        public float? AtmosphericVisibility => Visibility;

        /// <summary>
        /// A conversion of <see cref="OWMCoord"/> to Juniper's own internal Lat/Lng type.
        /// </summary>
        private LatLngPoint loc;

        /// <summary>
        /// A conversion of <see cref="OWMCoord"/> to Juniper's own internal Lat/Lng type.
        /// </summary>
        public LatLngPoint Location
        {
            get
            {
                if (loc is null && Coord is object)
                {
                    loc = new LatLngPoint(Coord.lat, Coord.lon, 0);
                }

                return loc;
            }
        }

        /// <summary>
        /// A short-form description of the weather conditions, achieved by joining the individual
        /// weather condition description values in a comma-delimited list.
        /// </summary>
        /// <value>The conditions.</value>
        public string Conditions
        {
            get
            {
                if (Weather is null)
                {
                    return "N/A";
                }
                else
                {
                    return string.Join(", ", (from w in Weather
                                              select w.description).ToArray());
                }
            }
        }

        /// <summary>
        /// If <see cref="Main"/> is not null, returns the current temperature from it.
        /// </summary>
        /// <value>The temperature.</value>
        public float? Temperature => Main?.temp;

        /// <summary>
        /// If <see cref="Main"/> is not null, returns the current atmospheric pressure from it.
        /// </summary>
        /// <value>The atmospheric pressure.</value>
        public float? AtmosphericPressure => Main?.pressure;

        /// <summary>
        /// If <see cref="Main"/> is not null, returns the current humidity from it.
        /// </summary>
        /// <value>The humidity.</value>
        public float? Humidity => Main?.humidity / 100f;

        /// <summary>
        /// If <see cref="Wind"/> is not null, returns the direction from it.
        /// </summary>
        /// <value>The wind direction.</value>
        public float? WindDirection => Wind?.deg;

        /// <summary>
        /// if <see cref="Wind"/> is not null, returns the speed from it.
        /// </summary>
        /// <value>The wind speed.</value>
        public float? WindSpeed => Wind?.speed;

        /// <summary>
        /// If <see cref="Clouds"/> is not null, returns the cloud cover value, scaled to the range
        /// [0, 1].
        /// </summary>
        /// <value>The cloud cover.</value>
        public float? CloudCover => Clouds?.all / 100f;

        /// <summary>
        /// If <see cref="Sys"/> is not null, returns the country value from it.
        /// </summary>
        /// <value>The country.</value>
        public string Country => Sys?.country;

        /// <summary>
        /// If <see cref="Sys"/> is not null, returns the SunriseTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunriseTime => Sys?.SunriseTime;

        /// <summary>
        /// If <see cref="Sys"/> is not null, returns the SunsetTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunsetTime => Sys?.SunsetTime;
    }
}