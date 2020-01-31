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
                if(ReferenceEquals(this, AnyApplication))
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
