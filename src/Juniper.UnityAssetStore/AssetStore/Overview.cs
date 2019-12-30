using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Overview<T> : ISerializable
        where T : class, ISerializable
    {
        public readonly T overview;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Overview(SerializationInfo info, StreamingContext context)
        {
            overview = info.GetValue<T>(nameof(overview));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(overview), overview);
        }
    }
}