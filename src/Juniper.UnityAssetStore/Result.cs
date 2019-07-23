using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Result<T> : ISerializable
        where T : ISerializable
    {
        public readonly T result;
        public readonly string error;

        protected Result(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(error):
                    error = info.GetString(nameof(error));
                    break;

                    case nameof(result):
                    result = info.GetValue<T>(nameof(result));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(result), result);
        }
    }
}