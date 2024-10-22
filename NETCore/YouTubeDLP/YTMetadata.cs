namespace Juniper;

public class YTMetadata
{
    public required string id { get; set; }
    public required YTMetadataFormat[] formats { get; set; }
    public required YTMetadataFormat[] requested_formats { get; set; }
    public required YTMetadataThumbnail[] thumbnails { get; set; }
    public required YTMetadataPlaylist[] playlist { get; set; }
    public required string title { get; set; }
    public required string thumbnail { get; set; }
    public required string description { get; set; }
    public required string upload_date { get; set; }
    public required string uploader { get; set; }
    public required string uploader_id { get; set; }
    public required string uploader_url { get; set; }
    public required string channel_id { get; set; }
    public required string channel_url { get; set; }
    public float? duration { get; set; }
    public int? view_count { get; set; }
    public float? average_rating { get; set; }
    public int? age_limit { get; set; }
    public required string webpage_url { get; set; }
    public required string[] categories { get; set; }
    public required string[] tags { get; set; }
    public bool? playable_in_embed { get; set; }
    public bool? is_live { get; set; }
    public bool? was_live { get; set; }
    public required string live_status { get; set; }
    public float? release_timestamp { get; set; }
    public required object chapters { get; set; }
    public int? like_count { get; set; }
    public required string channel { get; set; }
    public int? channel_follower_count { get; set; }
    public required string track { get; set; }
    public required string artist { get; set; }
    public required string creator { get; set; }
    public required string alt_title { get; set; }
    public required string availability { get; set; }
    public required string original_url { get; set; }
    public required string webpage_url_basename { get; set; }
    public required string webpage_url_domain { get; set; }
    public required string extractor { get; set; }
    public required string extractor_key { get; set; }
    public int? playlist_index { get; set; }
    public required string display_id { get; set; }
    public required string duration_string { get; set; }
    public bool? requested_subtitles { get; set; }
    public bool? __has_drm { get; set; }
    public required string fulltitle { get; set; }
    public required string format { get; set; }
    public required string format_id { get; set; }
    public required string ext { get; set; }
    public required string protocol { get; set; }
    public required string language { get; set; }
    public required string format_note { get; set; }
    public float? filesize_approx { get; set; }
    public float? tbr { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public required string resolution { get; set; }
    public float? fps { get; set; }
    public required string dynamic_range { get; set; }
    public required string vcodec { get; set; }
    public float? vbr { get; set; }
    public float? stretched_ratio { get; set; }
    public required string acodec { get; set; }
    public float? abr { get; set; }
    public int? asr { get; set; }
    public int? epoch { get; set; }
    public required string _filename { get; set; }
    public required string filename { get; set; }
    public required string urls { get; set; }
}