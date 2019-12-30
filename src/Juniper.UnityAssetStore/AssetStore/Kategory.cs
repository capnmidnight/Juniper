using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Kategory : ISerializable
    {
        public readonly string name;
        public readonly string slug;
        public readonly string id;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Kategory(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString(nameof(name));
            slug = info.GetString(nameof(slug));
            id = info.GetString(nameof(id));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(slug), slug);
            info.AddValue(nameof(id), id);
        }
    }
}