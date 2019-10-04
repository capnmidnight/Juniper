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
}