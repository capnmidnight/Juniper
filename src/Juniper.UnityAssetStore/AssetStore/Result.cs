using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Result<T> : ISerializable
        where T : class, ISerializable
    {
        public readonly T result;
        public readonly string error;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Result(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

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
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(result), result);
        }
    }
}