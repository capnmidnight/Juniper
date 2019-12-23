using System;
using System.Runtime.Serialization;

namespace Juniper.World.Climate.OpenWeatherMap
{
    /// <summary>
    /// The current cloud cover.
    /// </summary>
    [Serializable]
    public class OWMClouds : ISerializable
    {
        /// <summary>
        /// An estimate, from 0 to 100, of the current cloud cover.
        /// </summary>
        public int all;

        /// <summary>
        /// Deserializes a Clouds.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected OWMClouds(SerializationInfo info, StreamingContext context)
        {
            all = info.GetInt32(nameof(all));
        }

        /// <summary>
        /// Serializes the Clouds.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(all), all);
        }
    }
}