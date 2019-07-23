using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
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
}