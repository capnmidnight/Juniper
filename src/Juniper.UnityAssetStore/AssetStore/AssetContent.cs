using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class AssetContents : ISerializable
    {
        public AssetContent[] assets;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected AssetContents(SerializationInfo info, StreamingContext context)
        {
            assets = info.GetValue<AssetContent[]>(nameof(assets));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(assets), assets);
        }
    }

    [Serializable]
    public class AssetContent : ISerializable
    {
        public readonly int level;
        public readonly string asset_id;
        public readonly string guid;
        public readonly string label;
        public readonly int folder;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected AssetContent(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(level):
                    level = info.GetInt32(nameof(level));
                    break;

                    case nameof(label):
                    label = info.GetString(nameof(label));
                    break;

                    case nameof(folder):
                    folder = info.GetInt32(nameof(folder));
                    break;

                    case nameof(asset_id):
                    asset_id = info.GetString(nameof(asset_id));
                    break;

                    case nameof(guid):
                    guid = info.GetString(nameof(guid));
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(level), level);
            info.AddValue(nameof(label), label);
            info.AddValue(nameof(folder), folder);

            if (asset_id != null)
            {
                info.AddValue(nameof(asset_id), asset_id);
            }

            if (guid != null)
            {
                info.AddValue(nameof(guid), guid);
            }
        }
    }
}