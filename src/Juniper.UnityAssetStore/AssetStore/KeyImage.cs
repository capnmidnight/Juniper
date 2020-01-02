using System;
using System.Runtime.Serialization;

namespace Juniper.UnityAssetStore
{
    [Serializable]
    public class KeyImage : ISerializable
    {
        public readonly string package_version_id;

        private readonly string small;
        public Uri Small { get; }

        private readonly string small_legacy;
        public Uri SmallLegacy { get; }

        private readonly string medium;
        public Uri Medium { get; }

        private readonly string medium_legacy;
        public Uri MediumLegacy { get; }

        private readonly string big;
        public Uri Big { get; }

        private readonly string big_legacy;
        public Uri BigLegacy { get; }

        private readonly string icon;
        public Uri Icon { get; }

        private readonly string icon24;
        public Uri Icon24 { get; }

        private readonly string icon25;
        public Uri Icon25 { get; }

        private readonly string icon75;
        public Uri Icon75 { get; }

        private readonly string facebook;
        public Uri Facebook { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Context parameter is required by ISerializable interface.")]
        protected KeyImage(SerializationInfo info, StreamingContext context)
        {
            package_version_id = null;
            small = null;
            small_legacy = null;
            medium = null;
            medium_legacy = null;
            big = null;
            big_legacy = null;
            icon = null;
            icon24 = null;
            icon25 = null;
            icon75 = null;
            facebook = null;

            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(package_version_id):
                    package_version_id = info.GetString(nameof(package_version_id));
                    break;

                    case nameof(small):
                    small = info.GetString(nameof(small));
                    if (!string.IsNullOrWhiteSpace(small))
                    {
                        Small = new Uri(small);
                    }
                    break;

                    case nameof(small_legacy):
                    small_legacy = info.GetString(nameof(small_legacy));
                    if (!string.IsNullOrWhiteSpace(small_legacy))
                    {
                        SmallLegacy = new Uri(small_legacy);
                    }
                    break;

                    case nameof(medium):
                    medium = info.GetString(nameof(medium));
                    if (!string.IsNullOrWhiteSpace(medium))
                    {
                        Medium = new Uri(medium);
                    }
                    break;

                    case nameof(medium_legacy):
                    medium_legacy = info.GetString(nameof(medium_legacy));
                    if (!string.IsNullOrWhiteSpace(medium_legacy))
                    {
                        MediumLegacy = new Uri(medium_legacy);
                    }
                    break;

                    case nameof(big):
                    big = info.GetString(nameof(big));
                    if (!string.IsNullOrWhiteSpace(big))
                    {
                        Big = new Uri(big);
                    }
                    break;

                    case nameof(big_legacy):
                    big_legacy = info.GetString(nameof(big_legacy));
                    if (!string.IsNullOrWhiteSpace(big_legacy))
                    {
                        BigLegacy = new Uri(big_legacy);
                    }
                    break;

                    case nameof(icon):
                    icon = info.GetString(nameof(icon));
                    if (!string.IsNullOrWhiteSpace(icon))
                    {
                        Icon = new Uri(icon);
                    }
                    break;

                    case nameof(icon24):
                    icon24 = info.GetString(nameof(icon24));
                    if (!string.IsNullOrWhiteSpace(icon24))
                    {
                        Icon24 = new Uri(icon24);
                    }
                    break;

                    case nameof(icon25):
                    icon25 = info.GetString(nameof(icon25));
                    if (!string.IsNullOrWhiteSpace(icon25))
                    {
                        Icon25 = new Uri(icon25);
                    }
                    break;

                    case nameof(icon75):
                    icon75 = info.GetString(nameof(icon75));
                    if (!string.IsNullOrWhiteSpace(icon75))
                    {
                        Icon75 = new Uri(icon75);
                    }
                    break;

                    case nameof(facebook):
                    facebook = info.GetString(nameof(facebook));
                    if (!string.IsNullOrWhiteSpace(facebook))
                    {
                        Facebook = new Uri(facebook);
                    }
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (package_version_id is object)
            {
                info.AddValue(nameof(package_version_id), package_version_id);
            }

            if (small is object)
            {
                info.AddValue(nameof(small), small);
            }

            if (small_legacy is object)
            {
                info.AddValue(nameof(small_legacy), small_legacy);
            }

            if (medium is object)
            {
                info.AddValue(nameof(medium), medium);
            }

            if (big is object)
            {
                info.AddValue(nameof(big), big);
            }

            if (big_legacy is object)
            {
                info.AddValue(nameof(big_legacy), big_legacy);
            }

            if (icon is object)
            {
                info.AddValue(nameof(icon), icon);
            }

            if (icon24 is object)
            {
                info.AddValue(nameof(icon24), icon24);
            }

            if (icon25 is object)
            {
                info.AddValue(nameof(icon25), icon25);
            }

            if (icon75 is object)
            {
                info.AddValue(nameof(icon75), icon75);
            }

            if (facebook is object)
            {
                info.AddValue(nameof(facebook), facebook);
            }
        }
    }
}