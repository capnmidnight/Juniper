using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
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
}