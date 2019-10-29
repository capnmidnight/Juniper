using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
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
        /// A Unix time-stamp for when sunrise should occur for the day.
        /// </summary>
        public long sunrise;

        /// <summary>
        /// A Unix time-stamp for when sunset should occur for the day.
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
        /// Converts the Unix time-stamp value of <see cref="sunrise"/> to a DateTime value stored
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
        /// Converts the Unix time-stamp value of <see cref="sunset"/> to a DateTime value stored
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