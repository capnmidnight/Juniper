using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Multipart : MediaType
        {
            private Multipart(string value, string[] extensions) : base("multipart/" + value, extensions) { }

            private Multipart(string value) : this(value, null) { }

            public static readonly Multipart AnyMultipart = new Multipart("*");

            public override bool Matches(string fileName)
            {
                if (ReferenceEquals(this, AnyMultipart))
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
