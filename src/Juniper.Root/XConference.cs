namespace Juniper
{
    public partial class MediaType
    {
        public partial class XConference : MediaType
        {
            private static List<XConference>? _allXConf;
            public static List<XConference> AllXConf => _allXConf ??= new();
            public static IReadOnlyCollection<XConference> AllXConference => AllXConf;
            public static readonly XConference AnyXConference = new("*");

            public XConference(string value, params string[] extensions) : base("xconference", value, extensions)
            {
                if (SubType != "*")
                {
                    AllXConf.Add(this);
                }
            }

            public XConference(MediaType copy)
                : this(
                      copy.Type == "xconference"
                      ? copy.FullSubType
                      : throw new ArgumentException("Invalid media type", nameof(copy)),
                      copy.Extensions.ToArray())
            {

            }
        }
    }
}
