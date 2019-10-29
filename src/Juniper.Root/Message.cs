using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Message : MediaType
        {
            private Message(string value, string[] extensions) : base("message/" + value, extensions) {}

            private Message(string value) : this(value, null) {}

            public static readonly Message AnyMessage = new Message("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyMessage
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
