namespace Juniper;

public class YTMetadataFormat : YTMetadataURL
{
    public required string format_id { get; set; }
    public required string format_note { get; set; }
    public required string ext { get; set; }
    public required string protocol { get; set; }
    public required string acodec { get; set; }
    public required string vcodec { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public required YTMetadataFormatFragment[] fragments { get; set; }
    public required string audio_ext { get; set; }
    public required string video_ext { get; set; }
    public required string format { get; set; }
    public required string resolution { get; set; }
    public required IDictionary<string, string> http_headers { get; set; }
    public int? asr { get; set; }
    public int? filesize { get; set; }
    public int? source_preference { get; set; }
    public float? fps { get; set; }
    public float? quality { get; set; }
    public float? tbr { get; set; }
    public required string language { get; set; }
    public int? language_preference { get; set; }
    public required string dynamic_range { get; set; }
    public float? abr { get; set; }

    public required YTMetadataDownloaderOptions downloader_options { get; set; }

    public required string container { get; set; }
    public float? vbr { get; set; }
    public float? filesize_approx { get; set; }
}