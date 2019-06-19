using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Rating : ISerializable
    {
        public readonly int count;
        public readonly double average;

        public Rating(SerializationInfo info, StreamingContext context)
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
