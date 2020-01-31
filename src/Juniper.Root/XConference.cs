using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class XConference : MediaType
        {
            private XConference(string value, string[] extensions) : base("xconference/" + value, extensions) { }

            private XConference(string value) : this(value, null) { }

            public static readonly XConference AnyXConference = new XConference("*");

            public override bool Matches(string fileName)
            {
                if (ReferenceEquals(this, AnyXConference))
                {
                    return Values.Any(x => x.Matches(fileName));
                }
                else
                {
                    return base.Matches(fileName);
                }
            }
        }
    }
}
