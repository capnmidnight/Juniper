using Juniper.World.GIS;

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// respone from the OpenWeatherMap API. <seealso cref="http://openweathermap.org/current#current_JSON"/>.
    /// </summary>
    [Serializable]
    public class WeatherReport : IWeatherReport, ISerializable
    {
        /// <summary>
        /// When not null, the error response from the server.
        /// </summary>
        public string error;

        /// <summary>
        /// response ID
        /// </summary>
        public int id;

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string name;

        /// <summary>
        /// timestamp for the period of time the report covers.
        /// </summary>
        public long dt;

        /// <summary>
        /// atmospheric visibility
        /// </summary>
        public int visibility;

        /// <summary>
        /// latitude/longitude for which the report covers.
        /// </summary>
        public OWMCoord coord;

        /// <summary>
        /// various weather conditions that the report shows.
        /// </summary>
        public OWMWeather[] weather;

        /// <summary>
        /// The main metrics portion of the weather forcast.
        /// </summary>
        public OWMMain main;

        /// <summary>
        /// The current wind speed and direction.
        /// </summary>
        public OWMWind wind;

        /// <summary>
        /// The current cloud cover.
        /// </summary>
        public OWMClouds clouds;

        /// <summary>
        /// The expected rainfall for the next three hours.
        /// </summary>
        public OWMRain rain;

        /// <summary>
        /// The expected snowfall for the next three hours.
        /// </summary>
        public OWMSnow snow;

        /// <summary>
        /// Additional geographic information about the weather report.
        /// </summary>
        public OWMSys sys;

        /// <summary>
        /// Deserializes a WeatherReport.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WeatherReport(SerializationInfo info, StreamingContext context)
        {
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
        public string ErrorMessage
        {
            get
            {
                return error;
            }
        }

        /// <summary>
        /// name of the city for which the report covers.
        /// </summary>
        public string City
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// timestamp for the period of time the report covers.
        /// </summary>
        private DateTime? repTime;

        /// <summary>
        /// timestamp for the period of time the report covers.
        /// </summary>
        public DateTime ReportTime
        {
            get
            {
                if (repTime == null)
                {
                    if (error == null)
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
        public float? AtmosphericVisibility
        {
            get
            {
                return visibility;
            }
        }

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
                if (loc == null && coord != null)
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
                if (weather == null)
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
        public float? Temperature
        {
            get
            {
                return main?.temp;
            }
        }

        /// <summary>
        /// If <see cref="main"/> is not null, returns the current atmospheric pressure from it.
        /// </summary>
        /// <value>The atmospheric pressure.</value>
        public float? AtmosphericPressure
        {
            get
            {
                return main?.pressure;
            }
        }

        /// <summary>
        /// If <see cref="main"/> is not null, returns the current humidity from it.
        /// </summary>
        /// <value>The humidity.</value>
        public float? Humidity
        {
            get
            {
                return main?.humidity / 100f;
            }
        }

        /// <summary>
        /// If <see cref="wind"/> is not null, returns the direction from it.
        /// </summary>
        /// <value>The wind direction.</value>
        public float? WindDirection
        {
            get
            {
                return wind?.deg;
            }
        }

        /// <summary>
        /// if <see cref="wind"/> is not null, returns the speed from it.
        /// </summary>
        /// <value>The wind speed.</value>
        public float? WindSpeed
        {
            get
            {
                return wind?.speed;
            }
        }

        /// <summary>
        /// If <see cref="clouds"/> is not null, returns the cloud cover value, scaled to the range
        /// [0, 1].
        /// </summary>
        /// <value>The cloud cover.</value>
        public float? CloudCover
        {
            get
            {
                return clouds?.all / 100f;
            }
        }

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the country value from it.
        /// </summary>
        /// <value>The country.</value>
        public string Country
        {
            get
            {
                return sys?.country;
            }
        }

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the SunriseTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunriseTime
        {
            get
            {
                return sys?.SunriseTime;
            }
        }

        /// <summary>
        /// If <see cref="sys"/> is not null, returns the SunsetTime value from it.
        /// </summary>
        /// <value>The country.</value>
        public DateTime? SunsetTime
        {
            get
            {
                return sys?.SunsetTime;
            }
        }

        /// <summary>
        /// Latitude and longitude values for the OpenWeatherMap report.
        /// </summary>
        [Serializable]
        public class OWMCoord : ISerializable
        {
            /// <summary>
            /// Longitude
            /// </summary>
            public float lon;

            /// <summary>
            /// Latitude
            /// </summary>
            public float lat;

            /// <summary>
            /// Deserializes a Coord.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMCoord(SerializationInfo info, StreamingContext context)
            {
                lon = info.GetSingle(nameof(lon));
                lat = info.GetSingle(nameof(lat));
            }

            /// <summary>
            /// Serializes the Coord.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(lon), lon);
                info.AddValue(nameof(lat), lat);
            }
        }

        /// <summary>
        /// nested portion of the OpenWeatherMap report that covers weather conditions.
        /// </summary>
        [Serializable]
        public class OWMWeather : ISerializable
        {
            /// <summary>
            /// main weather forecast.
            /// </summary>
            public string main;

            /// <summary>
            /// An extended description of the main weather forcast.
            /// </summary>
            public string description;

            /// <summary>
            /// name of the icon to use to go along with the weather forcast.
            /// </summary>
            public string icon;

            /// <summary>
            /// An enumeration of the current conditions.
            /// </summary>
            public int id;

            /// <summary>
            /// Deserializes a Wather.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMWeather(SerializationInfo info, StreamingContext context)
            {
                main = info.GetString(nameof(main));
                description = info.GetString(nameof(description));
                icon = info.GetString(nameof(icon));
                id = info.GetInt32(nameof(id));
            }

            /// <summary>
            /// Serializes the Weather.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(main), main);
                info.AddValue(nameof(description), description);
                info.AddValue(nameof(icon), icon);
                info.AddValue(nameof(id), id);
            }

            /// <summary>
            /// full URL to the <see cref="icon"/> on OpenWeatherMap's server.
            /// </summary>
            /// <value>The icon URL.</value>
            public string IconURL
            {
                get
                {
                    return $"http://openweathermap.org/img/w/{icon}.png";
                }
            }

            /// <summary>
            /// weather conditions enumeration holds values that can be combined as flags to indicate
            /// different weather issues.
            /// </summary>
            public enum WeatherConditions
            {
                /// <summary>
                /// thunderstorm with light rain.
                /// </summary>
                THUNDERSTORM_WITH_LIGHT_RAIN = 200,

                /// <summary>
                /// thunderstorm with moderate rain.
                /// </summary>
                THUNDERSTORM_WITH_RAIN = 201,

                /// <summary>
                /// thunderstorm with heavy rain.
                /// </summary>
                THUNDERSTORM_WITH_HEAVY_RAIN = 202,

                /// <summary>
                /// light thunderstorm.
                /// </summary>
                LIGHT_THUNDERSTORM = 210,

                /// <summary>
                /// thunderstorm.
                /// </summary>
                THUNDERSTORM = 211,

                /// <summary>
                /// heavy thunderstorm.
                /// </summary>
                HEAVY_THUNDERSTORM = 212,

                /// <summary>
                /// ragged thunderstorm.
                /// </summary>
                RAGGED_THUNDERSTORM = 221,

                /// <summary>
                /// thunderstorm with light drizzle.
                /// </summary>
                THUNDERSTORM_WITH_LIGHT_DRIZZLE = 230,

                /// <summary>
                /// thunderstorm with drizzle.
                /// </summary>
                THUNDERSTORM_WITH_DRIZZLE = 231,

                /// <summary>
                /// thunderstorm with heavy drizzle.
                /// </summary>
                THUNDERSTORM_WITH_HEAVY_DRIZZLE = 232,

                /// <summary>
                /// light intensity drizzle.
                /// </summary>
                LIGHT_INTENSITY_DRIZZLE = 300,

                /// <summary>
                /// drizzle.
                /// </summary>
                DRIZZLE = 301,

                /// <summary>
                /// heavy intensity drizzle.
                /// </summary>
                HEAVY_INTENSITY_DRIZZLE = 302,

                /// <summary>
                /// light intensity drizzle rain.
                /// </summary>
                LIGHT_INTENSITY_DRIZZLE_RAIN = 310,

                /// <summary>
                /// drizzle rain.
                /// </summary>
                DRIZZLE_RAIN = 311,

                /// <summary>
                /// heavy intensity drizzle rain.
                /// </summary>
                HEAVY_INTENSITY_DRIZZLE_RAIN = 312,

                /// <summary>
                /// shower rain and drizzle.
                /// </summary>
                SHOWER_RAIN_AND_DRIZZLE = 313,

                /// <summary>
                /// heavy shower rain and drizzle.
                /// </summary>
                HEAVY_SHOWER_RAIN_AND_DRIZZLE = 314,

                /// <summary>
                /// shower drizzle.
                /// </summary>
                SHOWER_DRIZZLE = 321,

                /// <summary>
                /// light rain.
                /// </summary>
                LIGHT_RAIN = 500,

                /// <summary>
                /// moderate rain.
                /// </summary>
                MODERATE_RAIN = 501,

                /// <summary>
                /// heavy intensity rain.
                /// </summary>
                HEAVY_INTENSITY_RAIN = 502,

                /// <summary>
                /// very heavy rain.
                /// </summary>
                VERY_HEAVY_RAIN = 503,

                /// <summary>
                /// extreme rain.
                /// </summary>
                EXTREME_RAIN = 504,

                /// <summary>
                /// freezing rain.
                /// </summary>
                FREEZING_RAIN = 511,

                /// <summary>
                /// light intensity shower rain.
                /// </summary>
                LIGHT_INTENSITY_SHOWER_RAIN = 520,

                /// <summary>
                /// shower rain.
                /// </summary>
                SHOWER_RAIN = 521,

                /// <summary>
                /// heavy intensity shower rain.
                /// </summary>
                HEAVY_INTENSITY_SHOWER_RAIN = 522,

                /// <summary>
                /// ragged shower rain.
                /// </summary>
                RAGGED_SHOWER_RAIN = 531,

                /// <summary>
                /// light snow.
                /// </summary>
                LIGHT_SNOW = 600,

                /// <summary>
                /// snow.
                /// </summary>
                SNOW = 601,

                /// <summary>
                /// heavy snow.
                /// </summary>
                HEAVY_SNOW = 602,

                /// <summary>
                /// sleet.
                /// </summary>
                SLEET = 611,

                /// <summary>
                /// shower sleet.
                /// </summary>
                SHOWER_SLEET = 612,

                /// <summary>
                /// light rain and snow.
                /// </summary>
                LIGHT_RAIN_AND_SNOW = 615,

                /// <summary>
                /// rain and snow.
                /// </summary>
                RAIN_AND_SNOW = 616,

                /// <summary>
                /// light shower snow.
                /// </summary>
                LIGHT_SHOWER_SNOW = 620,

                /// <summary>
                /// shower snow.
                /// </summary>
                SHOWER_SNOW = 621,

                /// <summary>
                /// heavy shower snow.
                /// </summary>
                HEAVY_SHOWER_SNOW = 622,

                /// <summary>
                /// mist.
                /// </summary>
                MIST = 701,

                /// <summary>
                /// smoke.
                /// </summary>
                SMOKE = 711,

                /// <summary>
                /// haze.
                /// </summary>
                HAZE = 721,

                /// <summary>
                /// sand dust whirls.
                /// </summary>
                SAND_DUST_WHIRLS = 731,

                /// <summary>
                /// fog.
                /// </summary>
                FOG = 741,

                /// <summary>
                /// sand.
                /// </summary>
                SAND = 751,

                /// <summary>
                /// dust.
                /// </summary>
                DUST = 761,

                /// <summary>
                /// volcanic ash.
                /// </summary>
                VOLCANIC_ASH = 762,

                /// <summary>
                /// squalls.
                /// </summary>
                SQUALLS = 771,

                /// <summary>
                /// tornado.
                /// </summary>
                TORNADO = 781,

                /// <summary>
                /// clear sky.
                /// </summary>
                CLEAR_SKY = 800,

                /// <summary>
                /// few clouds.
                /// </summary>
                FEW_CLOUDS = 801,

                /// <summary>
                /// scattered clouds.
                /// </summary>
                SCATTERED_CLOUDS = 802,

                /// <summary>
                /// broken clouds.
                /// </summary>
                BROKEN_CLOUDS = 803,

                /// <summary>
                /// overcast clouds.
                /// </summary>
                OVERCAST_CLOUDS = 804,

                /// <summary>
                /// extreme tornado.
                /// </summary>
                EXTREME_TORNADO = 900,

                /// <summary>
                /// extreme tropical storm.
                /// </summary>
                EXTREME_TROPICAL_STORM = 901,

                /// <summary>
                /// extreme hurricane.
                /// </summary>
                EXTREME_HURRICANE = 902,

                /// <summary>
                /// extreme cold.
                /// </summary>
                EXTREME_COLD = 903,

                /// <summary>
                /// extreme hot.
                /// </summary>
                EXTREME_HOT = 904,

                /// <summary>
                /// extreme windy.
                /// </summary>
                EXTREME_WINDY = 905,

                /// <summary>
                /// extreme hail.
                /// </summary>
                EXTREME_HAIL = 906,

                /// <summary>
                /// calm.
                /// </summary>
                CALM = 951,

                /// <summary>
                /// light breeze.
                /// </summary>
                LIGHT_BREEZE = 952,

                /// <summary>
                /// gentle breeze.
                /// </summary>
                GENTLE_BREEZE = 953,

                /// <summary>
                /// moderate breeze.
                /// </summary>
                MODERATE_BREEZE = 954,

                /// <summary>
                /// fresh breeze.
                /// </summary>
                FRESH_BREEZE = 955,

                /// <summary>
                /// strong breeze.
                /// </summary>
                STRONG_BREEZE = 956,

                /// <summary>
                /// high wind near gale.
                /// </summary>
                HIGH_WIND_NEAR_GALE = 957,

                /// <summary>
                /// gale.
                /// </summary>
                GALE = 958,

                /// <summary>
                /// severe gale.
                /// </summary>
                SEVERE_GALE = 959,

                /// <summary>
                /// storm.
                /// </summary>
                STORM = 960,

                /// <summary>
                /// violent storm.
                /// </summary>
                VIOLENT_STORM = 961,

                /// <summary>
                /// hurricane.
                /// </summary>
                HURRICANE = 962
            }

            /// <summary>
            /// An enumeration of the current conditions.
            /// </summary>
            private WeatherConditions? cond;

            /// <summary>
            /// An enumeration of the current conditions.
            /// </summary>
            /// <value>The conditions.</value>
            public WeatherConditions Conditions
            {
                get
                {
                    return cond ?? (cond = (WeatherConditions)id).Value;
                }
            }
        }

        /// <summary>
        /// The main metrics portion of the weather forcast.
        /// </summary>
        [Serializable]
        public class OWMMain : ISerializable
        {
            /// <summary>
            /// The current temperature.
            /// </summary>
            public float temp;

            /// <summary>
            /// The atmospheric pressure.
            /// </summary>
            public float pressure;

            /// <summary>
            /// The air humidity.
            /// </summary>
            public float humidity;

            /// <summary>
            /// The predicted low for the day (which may not agree with the current temperature).
            /// </summary>
            public float temp_min;

            /// <summary>
            /// The predicted high for the day (which may not agree with the current temperatue).
            /// </summary>
            public float temp_max;

            /// <summary>
            /// The predicted pressure at sea level for the day (which may not agree with the current
            /// atmospheric pressure).
            /// </summary>
            public float sea_level;

            /// <summary>
            /// The predicted pressure at ground level for the day (which may not agree with the
            /// current atmospheric pressure).
            /// </summary>
            public float grnd_level;

            /// <summary>
            /// Deserializes a Main conditions structure.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMMain(SerializationInfo info, StreamingContext context)
            {
                temp = info.GetSingle(nameof(temp));
                pressure = info.GetSingle(nameof(pressure));
                humidity = info.GetSingle(nameof(humidity));
                temp_min = info.GetSingle(nameof(temp_min));
                temp_max = info.GetSingle(nameof(temp_max));
                sea_level = info.GetSingle(nameof(sea_level));
                grnd_level = info.GetSingle(nameof(grnd_level));
            }

            /// <summary>
            /// Serializes the Main conditions structure.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(temp), temp);
                info.AddValue(nameof(pressure), pressure);
                info.AddValue(nameof(humidity), humidity);
                info.AddValue(nameof(temp_min), temp_min);
                info.AddValue(nameof(temp_max), temp_max);
                info.AddValue(nameof(sea_level), sea_level);
                info.AddValue(nameof(grnd_level), grnd_level);
            }
        }

        /// <summary>
        /// The current wind speed and direction.
        /// </summary>
        [Serializable]
        public class OWMWind : ISerializable
        {
            /// <summary>
            /// The current speed of the wind, in KMH.
            /// </summary>
            public float speed;

            /// <summary>
            /// The current direction of the wind, in degrees clockwise from north.
            /// </summary>
            public float deg;

            /// <summary>
            /// Deserializes a Wind.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMWind(SerializationInfo info, StreamingContext context)
            {
                speed = info.GetSingle(nameof(speed));
                deg = info.GetSingle(nameof(deg));
            }

            /// <summary>
            /// Serializes the Wind.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(speed), speed);
                info.AddValue(nameof(deg), deg);
            }
        }

        /// <summary>
        /// The current cloud cover.
        /// </summary>
        [Serializable]
        public class OWMClouds : ISerializable
        {
            /// <summary>
            /// An estimate, from 0 to 100, of the current cloud cover.
            /// </summary>
            public int all;

            /// <summary>
            /// Deserializes a Clouds.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMClouds(SerializationInfo info, StreamingContext context)
            {
                all = info.GetInt32(nameof(all));
            }

            /// <summary>
            /// Serializes the Clouds.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(all), all);
            }
        }

        /// <summary>
        /// The expected rainfall for the next three hours.
        /// </summary>
        [Serializable]
        public class OWMRain : ISerializable
        {
            /// <summary>
            /// The expected rainfall for the next three hours.
            /// </summary>
            public int threeHour;

            /// <summary>
            /// Deserializes a Rain.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMRain(SerializationInfo info, StreamingContext context)
            {
                threeHour = info.GetInt32("3h");
            }

            /// <summary>
            /// Serializes the Rain.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("3h", threeHour);
            }
        }

        /// <summary>
        /// The expected snowfall for the next three hours.
        /// </summary>
        [Serializable]
        public class OWMSnow : ISerializable
        {
            /// <summary>
            /// The expected snowfall for the next three hours.
            /// </summary>
            public int threeHour;

            /// <summary>
            /// Deserializes a Snow.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMSnow(SerializationInfo info, StreamingContext context)
            {
                threeHour = info.GetInt32("3h");
            }

            /// <summary>
            /// Serializes the Snow.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("3h", threeHour);
            }
        }

        /// <summary>
        /// Additional geographic information about the weather report.
        /// </summary>
        [Serializable]
        public class OWMSys : ISerializable
        {
            /// <summary>
            /// The country in which the weather report covers.
            /// </summary>
            public string country;

            /// <summary>
            /// A Unix timestamp for when sunrise should occur for the day.
            /// </summary>
            public long sunrise;

            /// <summary>
            /// A Unix timestamp for when sunset should occur for the day.
            /// </summary>
            public long sunset;

            /// <summary>
            /// Deserializes a System info structure.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            protected OWMSys(SerializationInfo info, StreamingContext context)
            {
                country = info.GetString(nameof(country));
                sunrise = info.GetInt64(nameof(sunrise));
                sunset = info.GetInt64(nameof(sunset));
            }

            /// <summary>
            /// Serializes the System info structure.
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(country), country);
                info.AddValue(nameof(sunrise), sunrise);
                info.AddValue(nameof(sunset), sunset);
            }

            /// <summary>
            /// The time sunrise should occur for the day.
            /// </summary>
            private DateTime? riseTime;

            /// <summary>
            /// Converts the Unix timestamp value of <see cref="sunrise"/> to a DateTime value stored
            /// in <see cref="riseTime"/>, and returns that value.
            /// </summary>
            /// <value>The sunrise time.</value>
            public DateTime SunriseTime
            {
                get
                {
                    return riseTime ?? (riseTime = sunrise.UnixTimestampToDateTime()).Value;
                }
            }

            /// <summary>
            /// The time sunset should occur for the day.
            /// </summary>
            private DateTime? setTime;

            /// <summary>
            /// Converts the Unix timestamp value of <see cref="sunset"/> to a DateTime value stored
            /// in <see cref="setTime"/>, and returns that value.
            /// </summary>
            /// <value>The sunset time.</value>
            public DateTime SunsetTime
            {
                get
                {
                    return setTime ?? (setTime = sunset.UnixTimestampToDateTime()).Value;
                }
            }
        }
    }
}
