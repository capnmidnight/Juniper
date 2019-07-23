using System;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class PublisherDetail : ISerializable
    {
        public readonly string organization_id;
        public readonly string name;

        private readonly string support_email;
        public MailAddress SupportEmail { get; private set; }

        public readonly string description;

        private readonly string url;
        public Uri Url { get; private set; }

        public readonly string id;

        private readonly string short_url;
        public Uri ShortUrl { get; private set; }

        private readonly string support_url;
        public Uri SupportUrl { get; private set; }

        public readonly LatestAsset latest;
        public readonly KeyImage keyimage;
        public readonly Rating rating;

        protected PublisherDetail(SerializationInfo info, StreamingContext context)
        {
            organization_id = info.GetString(nameof(organization_id));
            name = info.GetString(nameof(name));

            support_email = info.GetString(nameof(support_email));
            SupportEmail = new MailAddress(support_email);

            description = info.GetString(nameof(description));

            url = info.GetString(nameof(url));
            if (!string.IsNullOrWhiteSpace(url))
            {
                Url = new Uri(url);
            }

            id = info.GetString(nameof(id));

            short_url = info.GetString(nameof(short_url));
            if (!string.IsNullOrWhiteSpace(short_url))
            {
                ShortUrl = new Uri(short_url);
            }

            support_url = info.GetString(nameof(support_url));
            if (!string.IsNullOrWhiteSpace(support_url))
            {
                SupportUrl = new Uri(support_url);
            }

            latest = info.GetValue<LatestAsset>(nameof(latest));
            keyimage = info.GetValue<KeyImage>(nameof(keyimage));
            rating = info.GetValue<Rating>(nameof(rating));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(organization_id), organization_id);
            info.AddValue(nameof(name), name);
            info.AddValue(nameof(support_email), support_email);
            info.AddValue(nameof(description), description);
            info.AddValue(nameof(url), url);
            info.AddValue(nameof(id), id);
            info.AddValue(nameof(short_url), short_url);
            info.AddValue(nameof(support_url), support_url);

            info.AddValue(nameof(latest), latest);
            info.AddValue(nameof(keyimage), keyimage);
            info.AddValue(nameof(rating), rating);
        }

        [Serializable]
        public class PublisherSummary : ISerializable
        {
            public readonly string label_english;
            public readonly string slug;
            public readonly string url;
            public readonly string id;
            public readonly string label;
            public readonly string support_email;
            public readonly string support_url;

            protected PublisherSummary(SerializationInfo info, StreamingContext context)
            {
                label_english = info.GetString(nameof(label_english));
                slug = info.GetString(nameof(slug));
                url = info.GetString(nameof(url));
                id = info.GetString(nameof(id));
                label = info.GetString(nameof(label));
                support_email = info.GetString(nameof(support_email));
                support_url = info.GetString(nameof(support_url));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(label_english), label_english);
                info.AddValue(nameof(slug), slug);
                info.AddValue(nameof(url), url);
                info.AddValue(nameof(id), id);
                info.AddValue(nameof(label), label);
                info.AddValue(nameof(support_email), support_email);
                info.AddValue(nameof(support_url), support_url);
            }
        }

        [Serializable]
        public class LatestAsset : ISerializable
        {
            public readonly string icon;
            public readonly string pubdate;
            public readonly string status;
            public readonly string url;
            public readonly string package_version_id;
            public readonly string slug;
            public readonly string id;
            public readonly string version;
            public readonly int license;
            public readonly string title_english;
            public readonly string title;

            public readonly Kategory kategory;
            public readonly CategorySummary category;
            public readonly PublisherSummary publisher;
            public readonly Link link;
            public readonly Flags flags;
            public readonly KeyImage keyimage;
            public readonly Tag[] list;

            protected LatestAsset(SerializationInfo info, StreamingContext context)
            {
                icon = info.GetString(nameof(icon));
                pubdate = info.GetString(nameof(pubdate));
                status = info.GetString(nameof(status));
                url = info.GetString(nameof(url));
                package_version_id = info.GetString(nameof(package_version_id));
                slug = info.GetString(nameof(slug));
                id = info.GetString(nameof(id));
                version = info.GetString(nameof(version));
                license = info.GetInt32(nameof(license));
                title_english = info.GetString(nameof(title_english));
                title = info.GetString(nameof(title));

                kategory = info.GetValue<Kategory>(nameof(kategory));
                category = info.GetValue<CategorySummary>(nameof(category));
                publisher = info.GetValue<PublisherSummary>(nameof(publisher));
                link = info.GetValue<Link>(nameof(link));
                try
                {
                    flags = info.GetValue<Flags>(nameof(flags));
                }
                catch { }
                keyimage = info.GetValue<KeyImage>(nameof(keyimage));
                list = info.GetValue<Tag[]>(nameof(list));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(icon), icon);
                info.AddValue(nameof(pubdate), pubdate);
                info.AddValue(nameof(status), status);
                info.AddValue(nameof(url), url);
                info.AddValue(nameof(package_version_id), package_version_id);
                info.AddValue(nameof(slug), slug);
                info.AddValue(nameof(id), id);
                info.AddValue(nameof(version), version);
                info.AddValue(nameof(license), license);
                info.AddValue(nameof(title_english), title_english);
                info.AddValue(nameof(title), title);

                info.AddValue(nameof(kategory), kategory);
                info.AddValue(nameof(category), category);
                info.AddValue(nameof(publisher), publisher);
                info.AddValue(nameof(link), link);
                info.AddValue(nameof(flags), flags);
                info.AddValue(nameof(keyimage), keyimage);
                info.AddValue(nameof(list), list);
            }
        }

        [Serializable]
        public class CategorySummary : ISerializable
        {
            public readonly string label_english;
            public readonly string multiple;
            public readonly string id;
            public readonly string label;

            protected CategorySummary(SerializationInfo info, StreamingContext context)
            {
                label_english = info.GetString(nameof(label_english));
                multiple = info.GetString(nameof(multiple));
                id = info.GetString(nameof(id));
                label = info.GetString(nameof(label));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(label_english), label_english);
                info.AddValue(nameof(multiple), multiple);
                info.AddValue(nameof(id), id);
                info.AddValue(nameof(label), label);
            }
        }
    }
}