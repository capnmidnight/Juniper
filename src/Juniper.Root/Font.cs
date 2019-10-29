using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Font : MediaType
        {
            private Font(string value, string[] extensions) : base("font/" + value, extensions) {}

            private Font(string value) : this(value, null) {}

            public static readonly Font AnyFont = new Font("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyFont
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
