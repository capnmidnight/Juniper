namespace Juniper;

public class YTMetadataPlaylist
{
    public required string id { get; set; }
    public required string ext { get; set; }
    public required string title { get; set; }
    public required string description { get; set; }
    public float? duration { get; set; }
    public required string upload_date { get; set; }
    public required string uploader { get; set; }
    public required string uploader_id { get; set; }
    public required string uploader_url { get; set; }
}