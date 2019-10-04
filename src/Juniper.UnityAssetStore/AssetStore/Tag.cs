using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Tag : ISerializable
    {
        public readonly string name;
        public readonly string slug_v2;
        public readonly string slug;
        public readonly string overlay;

        protected Tag(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString(nameof(name));
            slug_v2 = info.GetString(nameof(slug_v2));
            slug = info.GetString(nameof(slug));
            overlay = info.GetString(nameof(overlay));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(slug_v2), slug_v2);
            info.AddValue(nameof(slug), slug);
            info.AddValue(nameof(overlay), overlay);
        }
    }
}