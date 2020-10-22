using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Font : MediaType
        {
            private Font(string value, string[] extensions) : base("font/" + value, extensions) { }

            private Font(string value) : this(value, null) { }

            public static readonly Font AnyFont = new Font("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyFont))
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
                if (ReferenceEquals(this, AnyFont))
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
