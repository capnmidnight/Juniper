using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class AssetContent : ISerializable
    {
        private static readonly string LEVEL_FIELD = nameof(Level).ToLowerInvariant();
        private static readonly string ASSET_ID_FIELD = nameof(Asset_ID).ToLowerInvariant();
        private static readonly string GUID_FIELD = nameof(Guid).ToLowerInvariant();
        private static readonly string LABEL_FIELD = nameof(Label).ToLowerInvariant();
        private static readonly string FOLDER_FIELD = nameof(Folder).ToLowerInvariant();

        public int Level { get; }
        public string Asset_ID { get; }
        public string Guid { get; }
        public string Label { get; }
        public int Folder { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected AssetContent(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            foreach (var field in info)
            {
                var fieldName = field.Name.ToLowerInvariant();

                if (fieldName == LEVEL_FIELD)
                {
                    Level = info.GetInt32(field.Name);
                }
                else if (fieldName == LABEL_FIELD)
                {
                    Label = info.GetString(field.Name);
                }
                else if (fieldName == FOLDER_FIELD)
                {
                    Folder = info.GetInt32(field.Name);
                }
                else if (fieldName == ASSET_ID_FIELD)
                {
                    Asset_ID = info.GetString(field.Name);
                }
                else if (fieldName == GUID_FIELD)
                {
                    Guid = info.GetString(field.Name);
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(LEVEL_FIELD, Level);
            info.AddValue(LABEL_FIELD, Label);
            info.AddValue(FOLDER_FIELD, Folder);

            if (Asset_ID is object)
            {
                info.AddValue(ASSET_ID_FIELD, Asset_ID);
            }

            if (Guid is object)
            {
                info.AddValue(GUID_FIELD, Guid);
            }
        }
    }
}