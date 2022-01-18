namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Font : MediaType
        {
            public static readonly Font Collection = new("collection", new string[] { "ttc" });
            public static readonly Font Otf = new("otf", new string[] { "otf" });
            public static readonly Font Sfnt = new("sfnt");
            public static readonly Font Ttf = new("ttf", new string[] { "ttf" });
            public static readonly Font Woff = new("woff", new string[] { "woff" });
            public static readonly Font Woff2 = new("woff2", new string[] { "woff2" });

            public static new readonly Font[] Values = {
                Collection,
                Otf,
                Sfnt,
                Ttf,
                Woff,
                Woff2
            };
        }
    }
}
