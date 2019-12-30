using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Link : ISerializable
    {
        public readonly string id;
        public readonly string type;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Link(SerializationInfo info, StreamingContext context)
        {
            id = info.GetString(nameof(id));
            type = info.GetString(nameof(type));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(type), type);
        }
    }
}