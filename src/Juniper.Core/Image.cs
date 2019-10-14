using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Image : MediaType
        {
            private Image(string value, string[] extensions) : base("image/" + value, extensions) {}

            private Image(string value) : this(value, null) {}

            public static readonly Image AnyImage = new Image("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyImage
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
