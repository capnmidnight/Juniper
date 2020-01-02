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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected OWMSnow(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            threeHour = info.GetInt32("3h");
        }

        /// <summary>
        /// Serializes the Snow.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("3h", threeHour);
        }
    }
}