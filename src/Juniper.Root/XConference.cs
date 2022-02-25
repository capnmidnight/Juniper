using System.Collections.Generic;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class XConference : MediaType
        {
            private static List<XConference> _allXConf;
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
        }
    }
}
