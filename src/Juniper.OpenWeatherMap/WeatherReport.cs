using System;
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
        public string error { get; set; }

        /// <summary>
        /// response ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// time-stamp for the period of time the report covers.
        /// </summary>
        public long dt { get; set; }

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        public int visibility { get; set; }

        /// <summary>
        /// latitude/longitude for which the report covers.
        /// </summary>
        public OWMCoord coord { get; set; }

        /// <summary>
        /// various weather conditions that the report shows.
        /// </summary>
        public OWMWeather[] weather { get; set; }

        /// <summary>
        /// The main metrics portion of the weather forecast.
        /// </summary>
        public OWMMain main { get; set; }

        /// <summary>
        /// The current wind speed and direction.
        /// </summary>
        public OWMWind wind { get; set; }

        /// <summary>
        /// The current cloud cover.
        /// </summary>
        public OWMClouds clouds { get; set; }

        /// <summary>
        /// The expected rainfall for the next three hours.
        /// </summary>
        public OWMRain rain { get; set; }

        /// <summary>
        /// The expected snowfall for the next three hours.
        /// </summary>
        public OWMSnow snow { get; set; }

        /// <summary>
        /// Additional geographic information about the weather report.
        /// </summary>
        public OWMSys sys { get; set; }

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

            error = info.GetString(nameof(error));
            id = info.GetInt32(nameof(id));
            name = info.GetString(nameof(name));
            dt = info.GetInt64(nameof(dt));
            visibility = info.GetInt32(nameof(visibility));
            coord = (OWMCoord)info.GetValue(nameof(coord), typeof(OWMCoord));
            weather = (OWMWeather[])info.GetValue(nameof(weather), typeof(OWMWeather[]));
            main = (OWMMain)info.GetValue(nameof(main), typeof(OWMMain));
            wind = (OWMWind)info.GetValue(nameof(wind), typeof(OWMWind));
            clouds = (OWMClouds)info.GetValue(nameof(clouds), typeof(OWMClouds));
            rain = (OWMRain)info.GetValue(nameof(rain), typeof(OWMRain));
            snow = (OWMSnow)info.GetValue(nameof(snow), typeof(OWMSnow));
            sys = (OWMSys)info.GetValue(nameof(sys), typeof(OWMSys));
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

            info.AddValue(nameof(error), error);
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(dt), dt);
            info.AddValue(nameof(visibility), visibility);
            info.AddValue(nameof(coord), coord);
            info.AddValue(nameof(weather), weather);
            info.AddValue(nameof(main), main);
            info.AddValue(nameof(wind), wind);
            info.AddValue(nameof(clouds), clouds);
            info.AddValue(nameof(rain), rain);
            info.AddValue(nameof(snow), snow);
            info.AddValue(nameof(sys), sys);
        }

        /// <summary>
        /// When not null, the error response from the server.
        /// </summary>
        public string ErrorMessage => error;

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string City => name;

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
                    if (error is null)
                    {
                        repTime = dt.UnixTimestampToDateTime();
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
        public float? AtmosphericVisibility => visibility;

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
                if (loc is null && coord is object)
                {
                    loc = new LatLngPoint(coord.lat, coord.lon, 0);
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
                if (weather is null)
                {
                    return "N/A";
                }
                else
                {
                    return string.Join(", ", (from w in weather
                                              select w.description).ToArray());
                }
            }
        }

        /// <summary>
        /// If <see cref="main"/> is not null, returns the current temperature from it.
        /// </summary>
        /// <value>The temperature.</value>
        public float? Temperature => main?.temp;

        /// <summary>
        /// If <see cref="main"/> is not null, returns the current atmospheric pressure from it.
        /// </summary>
        /// <value>The atmospheric pressure.</value>
        public float? AtmosphericPressure => main?.pressure;

        /// <summary>
        /// If <see cref="main"/> is not null, returns the current humidity from it.
        /// </summary>
        /// <value>The humidity.</value>
        public float? Humidity => main?.humidity / 100f;

        /// <summary>
        /// If <see cref="wind"/> is not null, returns the direction from it.
        /// </summary>
        /// <value>The wind direction.</value>
        public float? WindDirection => wind?.deg;

        /// <summary>
        /// if <see cref="wind"/> is not null, returns the speed from it.
        /// </summary>
        /// <value>The wind speed.</value>
        public float? WindSpeed => wind?.speed;

        /// <summary>
        /// If <see cref="clouds"/> is not null, returns the cloud cover value, scaled to the range
        /// [0, 1].
        /// </summary>
        /// <value>The cloud cover.</value>
        public float? CloudCover => clouds?.all / 100f;

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the country value from it.
        /// </summary>
        /// <value>The country.</value>
        public string Country => sys?.country;

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the SunriseTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunriseTime => sys?.SunriseTime;

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the SunsetTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunsetTime => sys?.SunsetTime;
    }
}