using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Message : MediaType
        {
            private Message(string value, string[] extensions) : base("message/" + value, extensions) { }

            private Message(string value) : this(value, null) { }

            public static readonly Message AnyMessage = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyMessage))
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
                if (ReferenceEquals(this, AnyMessage))
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
