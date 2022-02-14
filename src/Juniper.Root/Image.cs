using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Image : MediaType
        {
            private static List<Image> _allImage;
            private static List<Image> AllImg => _allImage ??= new();
            public static IReadOnlyCollection<Image> AllImage => AllImg;
            public static readonly Image AnyImage = new("*");

            internal Image(string value, params string[] extensions) : base("image", value, extensions)
            {
                if (SubType != "*")
                {
                    AllImg.Add(this);
                }
            }
        }
    }
}
