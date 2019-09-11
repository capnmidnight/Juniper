using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Content<T> : ISerializable
        where T : class, ISerializable
    {
        public readonly T content;
        public readonly string error;

        public readonly bool HasContent;
        public readonly bool IsDeprecated;
        public readonly bool NotFound;

        protected Content(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(error):
                    error = info.GetString(nameof(error));
                    IsDeprecated = error == "deprecated";
                    NotFound = error == "not found";
                    break;

                    case nameof(content):
                    HasContent = true;
                    content = info.GetValue<T>(nameof(content));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (HasContent)
            {
                info.AddValue(nameof(content), content);
            }

            if (!string.IsNullOrEmpty(error))
            {
                info.AddValue(nameof(error), error);
            }
        }
    }
}