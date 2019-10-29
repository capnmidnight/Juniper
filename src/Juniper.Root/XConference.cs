using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class XConference : MediaType
        {
            private XConference(string value, string[] extensions) : base("xconference/" + value, extensions) {}

            private XConference(string value) : this(value, null) {}

            public static readonly XConference AnyXConference = new XConference("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyXConference
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
