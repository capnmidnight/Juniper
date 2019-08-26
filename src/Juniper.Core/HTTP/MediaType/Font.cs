namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Font : MediaType
        {
            public Font(string value, string[] extensions = null) : base("font/" + value, extensions) {}

            public static readonly Font Collection = new Font("collection", new string[] {"ttc"});
            public static readonly Font Otf = new Font("otf", new string[] {"otf"});
            public static readonly Font Sfnt = new Font("sfnt");
            public static readonly Font Ttf = new Font("ttf", new string[] {"ttf"});
            public static readonly Font Woff = new Font("woff", new string[] {"woff"});
            public static readonly Font Woff2 = new Font("woff2", new string[] {"woff2"});

            public static readonly new Font[] Values = {
                Collection,
                Otf,
                Sfnt,
                Ttf,
                Woff,
                Woff2,
            };
        }
    }
}
