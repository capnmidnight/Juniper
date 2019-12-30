using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class AssetDetail : ISerializable
    {
        public readonly string pubdate;
        private readonly DateTime pubdate_iso;

        private readonly string min_unity_version;
        public Version MinimumUnityVersion { get; }

        private readonly string[] unity_versions;
        public Version[] UnityVersions { get; }

        private readonly string url;
        public Uri Url { get; }

        public readonly string package_version_id;
        public readonly string slug;
        public readonly string id;
        public readonly string sizetext;

        private readonly string version;
        public Version Version { get; }

        public readonly DateTime first_published_at;
        public readonly string description;
        public readonly int license;
        public readonly string title;
        public readonly string publishnotes;

        private readonly string short_url;
        public readonly Uri ShortUrl;

        public readonly Rating rating;
        public readonly Kategory kategory;
        public readonly CategorySummary category;
        public readonly PublisherSummary publisher;
        public readonly Tag[] list;
        public readonly Link link;
        public readonly Image[] images;
        public readonly KeyImage keyimage;
        public readonly Flags flags;
        // "upgrades": [],
        // "upgradables": [],

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected AssetDetail(SerializationInfo info, StreamingContext context)
        {
            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(pubdate):
                    pubdate = info.GetString(nameof(pubdate));
                    break;

                    case nameof(pubdate_iso):
                    pubdate_iso = info.GetDateTime(nameof(pubdate_iso));
                    break;

                    case nameof(min_unity_version):
                    min_unity_version = info.GetString(nameof(min_unity_version));
                    MinimumUnityVersion = Version.Parse(min_unity_version);
                    break;

                    case nameof(unity_versions):
                    unity_versions = info.GetValue<string[]>(nameof(unity_versions));
                    UnityVersions = unity_versions.Select(Version.Parse).ToArray();
                    break;

                    case nameof(url):
                    url = info.GetString(nameof(url));
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        Url = new Uri(url);
                    }
                    break;

                    case nameof(package_version_id):
                    package_version_id = info.GetString(nameof(package_version_id));
                    break;

                    case nameof(slug):
                    slug = info.GetString(nameof(slug));
                    break;

                    case nameof(id):
                    id = info.GetString(nameof(id));
                    break;

                    case nameof(sizetext):
                    sizetext = info.GetString(nameof(sizetext));
                    break;

                    case nameof(version):
                    version = info.GetString(nameof(version));
                    Version = Version.Parse(version);
                    break;

                    case nameof(first_published_at):
                    first_published_at = info.GetDateTime(nameof(first_published_at));
                    break;

                    case nameof(description):
                    description = info.GetString(nameof(description));
                    break;

                    case nameof(license):
                    license = info.GetInt32(nameof(license));
                    break;

                    case nameof(title):
                    title = info.GetString(nameof(title));
                    break;

                    case nameof(publishnotes):
                    publishnotes = info.GetString(nameof(publishnotes));
                    break;

                    case nameof(short_url):
                    short_url = info.GetString(nameof(short_url));
                    if (!string.IsNullOrWhiteSpace(short_url))
                    {
                        ShortUrl = new Uri(short_url);
                    }
                    break;

                    case nameof(rating):
                    rating = info.GetValue<Rating>(nameof(rating));
                    break;

                    case nameof(kategory):
                    kategory = info.GetValue<Kategory>(nameof(kategory));
                    break;

                    case nameof(category):
                    category = info.GetValue<CategorySummary>(nameof(category));
                    break;

                    case nameof(publisher):
                    publisher = info.GetValue<PublisherSummary>(nameof(publisher));
                    break;

                    case nameof(list):
                    list = info.GetValue<Tag[]>(nameof(list));
                    break;

                    case nameof(link):
                    link = info.GetValue<Link>(nameof(link));
                    break;

                    case nameof(images):
                    images = info.GetValue<Image[]>(nameof(images));
                    break;

                    case nameof(keyimage):
                    keyimage = info.GetValue<KeyImage>(nameof(keyimage));
                    break;

                    case nameof(flags):
                    try
                    {
                        flags = info.GetValue<Flags>(nameof(flags));
                    }
                    catch { }
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(pubdate), pubdate);
            info.AddValue(nameof(pubdate_iso), pubdate_iso);
            info.AddValue(nameof(min_unity_version), min_unity_version);
            info.AddValue(nameof(unity_versions), unity_versions);
            info.AddValue(nameof(url), url);
            info.AddValue(nameof(package_version_id), package_version_id);
            info.AddValue(nameof(slug), slug);
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(sizetext), sizetext);
            info.AddValue(nameof(version), version);
            info.AddValue(nameof(first_published_at), first_published_at);
            info.AddValue(nameof(description), description);
            info.AddValue(nameof(license), license);
            info.AddValue(nameof(title), title);
            info.AddValue(nameof(publishnotes), publishnotes);
            info.AddValue(nameof(short_url), short_url);

            info.AddValue(nameof(rating), rating);
            info.AddValue(nameof(kategory), kategory);
            info.AddValue(nameof(category), category);
            info.AddValue(nameof(publisher), publisher);
            info.AddValue(nameof(list), list);
            info.AddValue(nameof(link), link);
            info.AddValue(nameof(images), images);
            info.AddValue(nameof(keyimage), keyimage);
            info.AddValue(nameof(flags), flags);
        }

        [Serializable]
        public class CategorySummary : ISerializable
        {
            public readonly string tree_id;
            public readonly string multiple;
            public readonly string id;
            public readonly string label;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
            protected CategorySummary(SerializationInfo info, StreamingContext context)
            {
                tree_id = info.GetString(nameof(tree_id));
                multiple = info.GetString(nameof(multiple));
                id = info.GetString(nameof(id));
                label = info.GetString(nameof(label));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(tree_id), tree_id);
                info.AddValue(nameof(multiple), multiple);
                info.AddValue(nameof(id), id);
                info.AddValue(nameof(label), label);
            }
        }

        [Serializable]
        public class PublisherSummary : ISerializable
        {
            public readonly string support_email;
            public readonly string url;
            public readonly string slug;
            public readonly string label;
            public readonly string id;
            public readonly string support_url;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
            protected PublisherSummary(SerializationInfo info, StreamingContext context)
            {
                support_email = info.GetString(nameof(support_email));
                url = info.GetString(nameof(url));
                slug = info.GetString(nameof(slug));
                label = info.GetString(nameof(label));
                id = info.GetString(nameof(id));
                support_url = info.GetString(nameof(support_url));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(support_email), support_email);
                info.AddValue(nameof(url), url);
                info.AddValue(nameof(slug), slug);
                info.AddValue(nameof(label), label);
                info.AddValue(nameof(id), id);
                info.AddValue(nameof(support_url), support_url);
            }
        }

        [Serializable]
        public class Image : ISerializable
        {
            public readonly string link;
            public readonly string width;
            public readonly string type;
            public readonly string height;
            public readonly string thumb;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
            protected Image(SerializationInfo info, StreamingContext context)
            {
                link = info.GetString(nameof(link));
                width = info.GetString(nameof(width));
                type = info.GetString(nameof(type));
                height = info.GetString(nameof(height));
                thumb = info.GetString(nameof(thumb));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(link), link);
                info.AddValue(nameof(width), width);
                info.AddValue(nameof(type), type);
                info.AddValue(nameof(height), height);
                info.AddValue(nameof(thumb), thumb);
            }
        }
    }
}