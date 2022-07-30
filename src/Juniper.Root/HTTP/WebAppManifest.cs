#nullable enable

using System.Text.Json.Serialization;

namespace Juniper.HTTP
{
    public class WebAppManifestImage
    {
        public string Src { get; set; }

        public string Sizes { get; set; }

        public string? Type { get; set; }

        public string? Purpose { get; set; }
        public string? Platform { get; set; }
        public string? Label { get; set; }
    }

    public class WebAppManifestShortcut
    {
        public string Url { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public WebAppManifestImage[]? Icons { get; set; }
    }

    public class WebAppManifestProtocolHandler
    {
        public string Protocol { get; set; }

        public string Url { get; set; }
    }

    public class WebAppManifestRelatedApplication
    {
        public string Platform { get; set; }

        public string Url { get; set; }

        public string? Id { get; set; }
    }

    public class WebAppManifest
    {
        public string Name { get; set; }

        public WebAppManifestImage[] Icons { get; set; }

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public string? Lang { get; set; }

        public string? Scope { get; set; }

        public string? Display { get; set; }

        public string? Orientation { get; set; }

        [JsonPropertyName("display_override")]
        public string[]? DisplayOverride { get; set; }

        public string? Dir { get; set; }

        public string[]? Categories { get; set; }

        [JsonPropertyName("iarc_rating_id")]
        public string? IARCRatingID { get; set; }

        [JsonPropertyName("start_url")]
        public string? StartUrl { get; set; }

        [JsonPropertyName("theme_color")]
        public string? ThemeColor { get; set; }

        [JsonPropertyName("background_color")]
        public string? BackgroundColor { get; set; }

        public WebAppManifestShortcut[]? Shortcuts { get; set; }

        public WebAppManifestImage[]? Screenshots { get; set; }

        [JsonPropertyName("protocol_handlers")]
        public WebAppManifestProtocolHandler[]? ProtocolHandlers { get; set; }

        [JsonPropertyName("related_applications")]
        public WebAppManifestRelatedApplication[]? RelatedApplications { get; set; }

        [JsonPropertyName("prefer_related_applications")]
        public bool? PreferRelatedApplications { get; set; }
    }
}
