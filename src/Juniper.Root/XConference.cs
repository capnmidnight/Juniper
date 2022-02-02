using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class XConference : MediaType
        {
            private XConference(string value, string[] extensions) : base("xconference/" + value, extensions) { }

            private XConference(string value) : this(value, null) { }

            public static readonly XConference AnyXConference = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyXConference))
                {
                    return Values.Any(x => x.GuessMatches(fileName));
                }
                else
                {
                    return base.GuessMatches(fileName);
                }
            }

            public override bool Matches(string mimeType)
            {
                if (ReferenceEquals(this, AnyXConference))
                {
                    return Values.Any(x => x.Matches(mimeType));
                }
                else
                {
                    return base.Matches(mimeType);
                }
            }
        }
    }
}
