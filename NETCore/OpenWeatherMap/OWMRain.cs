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
        public int threeHour { get; set; }

        /// <summary>
        /// Deserializes a Rain.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected OWMRain(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            threeHour = info.GetInt32("3h");
        }

        /// <summary>
        /// Serializes the Rain.
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