using System;
using System.Runtime.Serialization;

using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Overview<T> : ISerializable
        where T : ISerializable
    {
        public readonly T overview;

        public Overview(SerializationInfo info, StreamingContext context)
        {
            overview = info.GetValue<T>(nameof(overview));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(overview), overview);
        }
    }
}
