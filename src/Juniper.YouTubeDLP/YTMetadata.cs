#nullable disable
#pragma warning disable IDE1006 // Naming Styles
namespace Juniper
{
    public class YTMetadata
    {
        public string id { get; set; }
        public YTMetadataFormat[] formats { get; set; }
        public YTMetadataFormat[] requested_formats { get; set; }
        public YTMetadataThumbnail[] thumbnails { get; set; }
        public YTMetadataPlaylist[] playlist { get; set; }
        public string title { get; set; }
        public string thumbnail { get; set; }
        public string description { get; set; }
        public string upload_date { get; set; }
        public string uploader { get; set; }
        public string uploader_id { get; set; }
        public string uploader_url { get; set; }
        public string channel_id { get; set; }
        public string channel_url { get; set; }
        public float duration { get; set; }
        public int view_count { get; set; }
        public float? average_rating { get; set; }
        public int age_limit { get; set; }
        public string webpage_url { get; set; }
        public string[] categories { get; set; }
        public string[] tags { get; set; }
        public bool playable_in_embed { get; set; }
        public bool is_live { get; set; }
        public bool was_live { get; set; }
        public string live_status { get; set; }
        public float? release_timestamp { get; set; }
        public object chapters { get; set; }
        public int like_count { get; set; }
        public string channel { get; set; }
        public int channel_follower_count { get; set; }
        public string track { get; set; }
        public string artist { get; set; }
        public string creator { get; set; }
        public string alt_title { get; set; }
        public string availability { get; set; }
        public string original_url { get; set; }
        public string webpage_url_basename { get; set; }
        public string webpage_url_domain { get; set; }
        public string extractor { get; set; }
        public string extractor_key { get; set; }
        public int? playlist_index { get; set; }
        public string display_id { get; set; }
        public string duration_string { get; set; }
        public bool? requested_subtitles { get; set; }
        public bool __has_drm { get; set; }
        public string fulltitle { get; set; }
        public string format { get; set; }
        public string format_id { get; set; }
        public string ext { get; set; }
        public string protocol { get; set; }
        public string language { get; set; }
        public string format_note { get; set; }
        public float filesize_approx { get; set; }
        public float tbr { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string resolution { get; set; }
        public float fps { get; set; }
        public string dynamic_range { get; set; }
        public string vcodec { get; set; }
        public float vbr { get; set; }
        public float? stretched_ratio { get; set; }
        public string acodec { get; set; }
        public float abr { get; set; }
        public int asr { get; set; }
        public int epoch { get; set; }
        public string _filename { get; set; }
        public string filename { get; set; }
        public string urls { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles