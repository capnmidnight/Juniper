namespace Juniper;

public class YTMetadataThumbnail : YTMetadataURL
{
    public required string id { get; set; }
    public int? preference { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public required string resolution { get; set; }
}