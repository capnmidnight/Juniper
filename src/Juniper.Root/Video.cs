using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Video : MediaType
        {
            private Video(string value, string[] extensions) : base("video/" + value, extensions) { }

            private Video(string value) : this(value, null) { }

            public static readonly Video AnyVideo = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyVideo))
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
                if (ReferenceEquals(this, AnyVideo))
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
