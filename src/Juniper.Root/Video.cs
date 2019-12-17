using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Video : MediaType
        {
            private Video(string value, string[] extensions) : base("video/" + value, extensions) { }

            private Video(string value) : this(value, null) { }

            public static readonly Video AnyVideo = new Video("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                    || (this == AnyVideo
                        && Values.Any(v =>
                            v.Matches(fileName)));
            }
        }
    }
}
