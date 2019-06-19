using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class AssetSummary : ISerializable
    {
        public readonly string category;
        public readonly string title;
        public readonly string publisher;

        public AssetSummary(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(category):
                    category = info.GetString(nameof(category));
                    break;

                    case nameof(title):
                    title = info.GetString(nameof(title));
                    break;

                    case nameof(publisher):
                    publisher = info.GetString(nameof(publisher));
                    break;
                }
            }
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (!string.IsNullOrEmpty(category))
            {
                info.AddValue(nameof(category), category);
            }

            if (!string.IsNullOrEmpty(title))
            {
                info.AddValue(nameof(title), title);
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                info.AddValue(nameof(publisher), publisher);
            }
        }
    }
}
