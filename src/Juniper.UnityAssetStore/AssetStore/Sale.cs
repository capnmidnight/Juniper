using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class Sale : ISerializable
    {
        public readonly string banner;

        private readonly string feed;
        public Uri FeedUrl { get; }

        public readonly string status;
        public readonly int days_left;
        public readonly int total;
        public readonly int remaining;
        public readonly string badge;
        public readonly string title;
        public readonly bool countdown;
        public readonly object[] results;

        protected Sale(SerializationInfo info, StreamingContext context)
        {
            banner = info.GetString(nameof(banner));

            feed = info.GetString(nameof(feed));
            if (!string.IsNullOrWhiteSpace(feed))
            {
                FeedUrl = new Uri(feed);
            }

            status = info.GetString(nameof(status));
            days_left = info.GetInt32(nameof(days_left));
            total = info.GetInt32(nameof(total));
            remaining = info.GetInt32(nameof(remaining));
            badge = info.GetString(nameof(badge));
            title = info.GetString(nameof(title));
            countdown = info.GetBoolean(nameof(countdown));
            results = info.GetValue<object[]>(nameof(results));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(banner), banner);
            info.AddValue(nameof(feed), feed);
            info.AddValue(nameof(status), status);
            info.AddValue(nameof(days_left), days_left);
            info.AddValue(nameof(total), total);
            info.AddValue(nameof(remaining), remaining);
            info.AddValue(nameof(badge), badge);
            info.AddValue(nameof(title), title);
            info.AddValue(nameof(countdown), countdown);
            info.AddValue(nameof(results), results);
        }
    }
}