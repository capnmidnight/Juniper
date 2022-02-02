using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Application : MediaType
        {
            private Application(string value, string[] extensions) : base("application/" + value, extensions) { }

            private Application(string value) : this(value, null) { }

            public static readonly Application AnyApplication = new("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyApplication))
                {
                    return Values.Any(x => x.GuessMatches(fileName));
                }
                else
                {
                    return base.GuessMatches(fileName);
                }
            }

            public override bool Matches(string mimeType)
            {
                if (ReferenceEquals(this, AnyApplication))
                {
                    return Values.Any(x => x.Matches(mimeType));
                }
                else
                {
                    return base.Matches(mimeType);
                }
            }
        }
    }
}
