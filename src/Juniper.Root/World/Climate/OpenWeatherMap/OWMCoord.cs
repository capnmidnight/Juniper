using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
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
        /// Deserializes a Coordinate.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected OWMCoord(SerializationInfo info, StreamingContext context)
        {
            lon = info.GetSingle(nameof(lon));
            lat = info.GetSingle(nameof(lat));
        }

        /// <summary>
        /// Serializes the Coordinate.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(lon), lon);
            info.AddValue(nameof(lat), lat);
        }
    }
}