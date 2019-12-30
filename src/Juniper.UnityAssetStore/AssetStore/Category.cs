using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Categories : ISerializable
    {
        public Category[] categories;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Categories(SerializationInfo info, StreamingContext context)
        {
            categories = info.GetValue<Category[]>(nameof(categories));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(categories), categories);
        }
    }

    [Serializable]
    public class Category : ISerializable
    {
        public readonly string id;
        public readonly string name;
        private readonly string count;
        public readonly Category[] subs;

        public int AssetCount { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected Category(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(id):
                    id = info.GetString(nameof(id));
                    break;

                    case nameof(name):
                    name = info.GetString(nameof(name));
                    break;

                    case nameof(count):
                    count = info.GetString(nameof(count));
                    if (int.TryParse(count, out var iCount))
                    {
                        AssetCount = iCount;
                    }
                    break;

                    case nameof(subs):
                    subs = info.GetValue<Category[]>(nameof(subs));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(count), count);

            if (subs != null)
            {
                info.AddValue(nameof(subs), subs);
            }
        }
    }
}