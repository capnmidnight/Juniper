namespace Juniper
{
    public partial class MediaType
    {
        public partial class Text : MediaType
        {
            private static List<Text>? _allText;
            private static List<Text> AllTxt => _allText ??= new();
            public static IReadOnlyCollection<Text> AllText => AllTxt;
            public static readonly Text AnyText = new("*");

            public Text(string value, params string[] extensions) : base("text", value, extensions)
            {
                if (SubType != "*")
                {
                    AllTxt.Add(this);
                }
            }

            public Text(MediaType copy)
                : this(
                      copy.Type == "text"
                      ? copy.FullSubType
                      : throw new ArgumentException("Invalid media type", nameof(copy)),
                      copy.Extensions.ToArray())
            {

            }
        }
    }
}
