using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Application : MediaType
        {
            private Application(string value, string[] extensions) : base("application/" + value, extensions) { }

            private Application(string value) : this(value, null) { }

            public static readonly Application AnyApplication = new Application("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                    || (this == AnyApplication
                        && Values.Any(v =>
                            v.Matches(fileName)));
            }
        }
    }
}
