using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Audio : MediaType
        {
            private Audio(string value, string[] extensions) : base("audio/" + value, extensions) { }

            private Audio(string value) : this(value, null) { }

            public static readonly Audio AnyAudio = new Audio("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyAudio))
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
                if (ReferenceEquals(this, AnyAudio))
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
