using System;
using System.Runtime.Serialization;

using Juniper.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class AssetDownload : ISerializable
    {
        public readonly string icon;
        public readonly string local_version_name;
        public readonly string status;
        public readonly int in_users_downloads;
        public readonly Kategory kategory;
        public readonly DateTime created_at;
        public readonly DateTime last_downloaded_at;
        public readonly string local_path;
        public readonly int can_comment;
        public readonly string slug;
        public readonly CategorySummary category;
        public readonly int can_download;
        public readonly string id;
        public readonly PublisherSummary publisher;
        public readonly string icon128;
        public readonly int user_rating;
        public readonly DateTime published_at;
        public readonly string name;
        public readonly int is_complete_project;
        public readonly DateTime purchased_at;
        public readonly DateTime updated_at;
        public readonly int can_update;
        public readonly string type;

        public AssetDownload(SerializationInfo info, StreamingContext context)
        {
            icon = info.GetString(nameof(icon));
            local_version_name = info.GetString(nameof(local_version_name));
            status = info.GetString(nameof(status));
            in_users_downloads = info.GetInt32(nameof(in_users_downloads));
            kategory = info.GetValue<Kategory>(nameof(kategory));
            created_at = info.GetDateTime(nameof(created_at));
            last_downloaded_at = info.GetDateTime(nameof(last_downloaded_at));
            local_path = info.GetString(nameof(local_path));
            can_comment = info.GetInt32(nameof(can_comment));
            slug = info.GetString(nameof(slug));
            category = info.GetValue<CategorySummary>(nameof(category));
            can_download = info.GetInt32(nameof(can_download));
            id = info.GetString(nameof(id));
            publisher = info.GetValue<PublisherSummary>(nameof(publisher));
            icon128 = info.GetString(nameof(icon128));
            user_rating = info.GetInt32(nameof(user_rating));
            published_at = info.GetDateTime(nameof(published_at));
            name = info.GetString(nameof(name));
            is_complete_project = info.GetInt32(nameof(is_complete_project));
            purchased_at = info.GetDateTime(nameof(purchased_at));
            updated_at = info.GetDateTime(nameof(updated_at));
            can_update = info.GetInt32(nameof(can_update));
            type = info.GetString(nameof(type));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(icon), icon);
            info.AddValue(nameof(local_version_name), local_version_name);
            info.AddValue(nameof(status), status);
            info.AddValue(nameof(in_users_downloads), in_users_downloads);
            info.AddValue(nameof(kategory), kategory);
            info.AddValue(nameof(created_at), created_at);
            info.AddValue(nameof(last_downloaded_at), last_downloaded_at);
            info.AddValue(nameof(local_path), local_path);
            info.AddValue(nameof(can_comment), can_comment);
            info.AddValue(nameof(slug), slug);
            info.AddValue(nameof(category), category);
            info.AddValue(nameof(can_download), can_download);
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(publisher), publisher);
            info.AddValue(nameof(icon128), icon128);
            info.AddValue(nameof(user_rating), user_rating);
            info.AddValue(nameof(published_at), published_at);
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(is_complete_project), is_complete_project);
            info.AddValue(nameof(purchased_at), purchased_at);
            info.AddValue(nameof(updated_at), updated_at);
            info.AddValue(nameof(can_update), can_update);
            info.AddValue(nameof(type), type);
        }
    }

    [Serializable]
    public class CategorySummary : ISerializable
    {
        public readonly string name;
        public readonly string id;

        public CategorySummary(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString(nameof(name));
            id = info.GetString(nameof(id));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(id), id);
        }
    }
}
