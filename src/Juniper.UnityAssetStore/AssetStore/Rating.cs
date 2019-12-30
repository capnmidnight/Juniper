using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Rating : ISerializable
    {
        public readonly int count;
        public readonly double average;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Rating(SerializationInfo info, StreamingContext context)
        {
            count = info.GetInt32(nameof(count));
            average = info.GetDouble(nameof(average));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(count), count);
            info.AddValue(nameof(average), average);
        }
    }
}