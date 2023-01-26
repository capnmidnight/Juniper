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

            public Video(string value, params string[] extensions) : base("video", value, extensions)
            {
                if (SubType != "*")
                {
                    AllVid.Add(this);
                }
            }

            public Video(MediaType copy)
                : this(
                      copy.Type == "video"
                      ? copy.FullSubType
                      : throw new ArgumentException("Invalid media type", nameof(copy)),
                      copy.Extensions.ToArray())
            {

            }
        }
    }
}
