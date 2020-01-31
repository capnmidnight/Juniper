using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Message : MediaType
        {
            private Message(string value, string[] extensions) : base("message/" + value, extensions) { }

            private Message(string value) : this(value, null) { }

            public static readonly Message AnyMessage = new Message("*");

            public override bool Matches(string fileName)
            {
                if (ReferenceEquals(this, AnyMessage))
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
