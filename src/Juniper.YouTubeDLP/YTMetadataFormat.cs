#nullable disable
#pragma warning disable IDE1006 // Naming Styles
namespace Juniper
{
    public class YTMetadataFormat : YTMetadataURL
    {
        public string format_id { get; set; }
        public string format_note { get; set; }
        public string ext { get; set; }
        public string protocol { get; set; }
        public string acodec { get; set; }
        public string vcodec { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public YTMetadataFormatFragment[] fragments { get; set; }
        public string audio_ext { get; set; }
        public string video_ext { get; set; }
        public string format { get; set; }
        public string resolution { get; set; }
        public IDictionary<string, string> http_headers { get; set; }
        public int? asr { get; set; }
        public int? filesize { get; set; }
        public int? source_preference { get; set; }
        public float? fps { get; set; }
        public int? quality { get; set; }
        public float? tbr { get; set; }
        public string language { get; set; }
        public int? language_preference { get; set; }
        public string dynamic_range { get; set; }
        public float? abr { get; set; }

        public YTMetadataDownloaderOptions downloader_options { get; set; }

        public string container { get; set; }
        public float? vbr { get; set; }
        public float? filesize_approx { get; set; }
    }
}

#pragma warning restore IDE1006 // Naming Styles