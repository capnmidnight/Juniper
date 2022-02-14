namespace Juniper
{
    public partial class MediaType
    {
        public static readonly Font Font_Collection = new("collection", new string[] { "ttc" });
        public static readonly Font Font_Otf = new("otf", new string[] { "otf" });
        public static readonly Font Font_Sfnt = new("sfnt");
        public static readonly Font Font_Ttf = new("ttf", new string[] { "ttf" });
        public static readonly Font Font_Woff = new("woff", new string[] { "woff" });
        public static readonly Font Font_Woff2 = new("woff2", new string[] { "woff2" });
    }
}
