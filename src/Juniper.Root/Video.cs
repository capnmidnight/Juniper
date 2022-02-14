using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Video : MediaType
        {
            private static List<Video> _allVideo;
            private static List<Video> AllVid => _allVideo ??= new();
            public static IReadOnlyCollection<Video> AllVideo => AllVid;
            public static readonly Video AnyVideo = new("*");

            internal Video(string value, params string[] extensions) : base("video", value, extensions)
            {
                if (SubType != "*")
                {
                    AllVid.Add(this);
                }
            }
        }
    }
}
