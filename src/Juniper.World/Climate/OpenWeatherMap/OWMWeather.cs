using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Juniper.Image;
using Juniper.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
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
        /// An extended description of the main weather forecast.
        /// </summary>
        public string description;

        /// <summary>
        /// name of the icon to use to go along with the weather forecast.
        /// </summary>
        public string icon;

        /// <summary>
        /// An enumeration of the current conditions.
        /// </summary>
        public int id;

        /// <summary>
        /// Deserializes a Weather.
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

        private static IDeserializer<ImageData> decoder;

        public async Task<ImageData> GetIcon()
        {
            if (decoder == null)
            {
                decoder = new Image.PNG.PngFactory();
            }
            var request = HttpWebRequestExt.Create(IconURL);
            using (var response = await request.Get())
            {
                return decoder.Deserialize(response);
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
}