#nullable disable
#pragma warning disable IDE1006 // Naming Styles
namespace Juniper
{
    public class YTMetadataThumbnail : YTMetadataURL
    {
        public string id { get; set; }
        public int? preference { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string resolution { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles