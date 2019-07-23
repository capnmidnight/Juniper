using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Title : ISerializable
    {
        public readonly string title;

        protected Title(SerializationInfo info, StreamingContext context)
        {
            title = info.GetString(nameof(title));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(title), title);
        }
    }
}