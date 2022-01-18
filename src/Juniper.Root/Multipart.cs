using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Multipart : MediaType
        {
            private Multipart(string value, string[] extensions) : base("multipart/" + value, extensions) { }

            private Multipart(string value) : this(value, null) { }

            public static readonly Multipart AnyMultipart = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyMultipart))
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
                if (ReferenceEquals(this, AnyMultipart))
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
