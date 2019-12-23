using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// The main metrics portion of the weather forecast.
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
        /// The predicted high for the day (which may not agree with the current temperature).
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
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
}