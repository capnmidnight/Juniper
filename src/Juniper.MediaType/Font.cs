namespace Juniper;

public partial class MediaType
{
    public partial class Font : MediaType
    {
        private static List<Font> ?_allFont;
        private static List<Font> AllFnt => _allFont ??= new();
        public static IReadOnlyCollection<Font> AllFont => AllFnt;
        public static readonly Font AnyFont = new("*");

        public Font(string value, params string[] extensions) : base("font", value, extensions)
        {
            if (SubType != "*")
            {
                AllFnt.Add(this);
            }
        }

        public Font(MediaType copy)
            : this(
                  copy.Type == "font"
                  ? copy.FullSubType
                  : throw new ArgumentException("Invalid media type", nameof(copy)),
                  copy.Extensions.ToArray())
        {

        }
    }
}
