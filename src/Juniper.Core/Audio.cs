using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Audio : MediaType
        {
            private Audio(string value, string[] extensions) : base("audio/" + value, extensions) {}

            private Audio(string value) : this(value, null) {}

            public static readonly Audio AnyAudio = new Audio("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyAudio
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
