using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Image : MediaType
        {
            private Image(string value, string[] extensions) : base("image/" + value, extensions) { }

            private Image(string value) : this(value, null) { }

            public static readonly Image AnyImage = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyImage))
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
                if (ReferenceEquals(this, AnyImage))
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
