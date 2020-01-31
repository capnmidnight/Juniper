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
                if (ReferenceEquals(this, AnyVideo))
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
