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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Tag(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            name = info.GetString(nameof(name));
            slug_v2 = info.GetString(nameof(slug_v2));
            slug = info.GetString(nameof(slug));
            overlay = info.GetString(nameof(overlay));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(name), name);
            info.AddValue(nameof(slug_v2), slug_v2);
            info.AddValue(nameof(slug), slug);
            info.AddValue(nameof(overlay), overlay);
        }
    }
}