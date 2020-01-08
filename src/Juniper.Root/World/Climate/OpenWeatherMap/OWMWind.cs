using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// The current wind speed and direction.
    /// </summary>
    [Serializable]
    public class OWMWind : ISerializable
    {
        /// <summary>
        /// The current speed of the wind, in KMH.
        /// </summary>
        public float speed { get; set; }

        /// <summary>
        /// The current direction of the wind, in degrees clockwise from north.
        /// </summary>
        public float deg { get; set; }

        /// <summary>
        /// Deserializes a Wind.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected OWMWind(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(speed), speed);
            info.AddValue(nameof(deg), deg);
        }
    }
}