using System;
using System.Runtime.Serialization;

using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Results<T> : ISerializable
        where T : ISerializable
    {
        public readonly int total;
        public readonly string feed;
        public readonly bool HasResults;
        public readonly T[] results;

        protected Results(SerializationInfo info, StreamingContext context)
        {
            total = -1;
            feed = null;
            HasResults = false;

            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(total):
                    total = info.GetInt32(nameof(total));
                    break;
                    case nameof(feed):
                    feed = info.GetString(nameof(feed));
                    break;
                    case nameof(results):
                    case "result":
                    HasResults = true;
                    results = info.GetValue<T[]>(nameof(results));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(total >= 0)
            {
                info.AddValue(nameof(total), total);
            }

            if(!string.IsNullOrEmpty(feed))
            {
                info.AddValue(nameof(feed), feed);
            }

            if (HasResults)
            {
                info.AddValue(nameof(results), results);
            }
        }
    }
}
