namespace Juniper
{
    public partial class MediaType
    {
        public partial class Audio : MediaType
        {
            private static List<Audio> _allAudio;
            private static List<Audio> AllAud => _allAudio ??= new();
            public static IReadOnlyCollection<Audio> AllAudio => AllAud;
            public static readonly Audio AnyAudio = new("*");

            public Audio(string value, params string[] extensions) : base("audio", value, extensions)
            {
                if (SubType != "*")
                {
                    AllAud.Add(this);
                }
            }

            internal Audio(string[] extensions) : base("application", "octet-stream", extensions)
            {
                AllAud.Add(this);
            }
        }
    }
}
