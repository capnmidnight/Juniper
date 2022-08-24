using System.Text.Json.Serialization;

namespace Juniper.HTTP
{
    public class WebAppManifestImage
    {
        [JsonPropertyName("src")]
        public string Source { get; set; }

        [JsonPropertyName("sizes")]
        public string Sizes { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
    }

    public class WebAppManifestShortcut
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("icons")]
        public WebAppManifestImage[] Icons { get; set; }
    }

    public class WebAppManifestProtocolHandler
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class WebAppManifestRelatedApplication
    {
        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }
    }

    public class WebAppManifest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icons")]
        public WebAppManifestImage[] Icons { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("lang")]
        public string Language { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("display")]
        public string Display { get; set; }

        [JsonPropertyName("orientation")]
        public string Orientation { get; set; }

        [JsonPropertyName("display_override")]
        public string[] DisplayOverride { get; set; }

        [JsonPropertyName("dir")]
        public string TextDirection { get; set; }

        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }

        [JsonPropertyName("iarc_rating_id")]
        public string IARCRatingID { get; set; }

        [JsonPropertyName("start_url")]
        public string StartUrl { get; set; }

        [JsonPropertyName("theme_color")]
        public string ThemeColor { get; set; }

        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("ovr_package_name")]
        public string OVRPackageName { get; set; }

        [JsonPropertyName("shortcuts")]
        public WebAppManifestShortcut[] Shortcuts { get; set; }

        [JsonPropertyName("screenshots")]
        public WebAppManifestImage[] Screenshots { get; set; }

        [JsonPropertyName("protocol_handlers")]
        public WebAppManifestProtocolHandler[] ProtocolHandlers { get; set; }

        [JsonPropertyName("related_applications")]
        public WebAppManifestRelatedApplication[] RelatedApplications { get; set; }

        [JsonPropertyName("prefer_related_applications")]
        public bool? PreferRelatedApplications { get; set; }
    }
}
