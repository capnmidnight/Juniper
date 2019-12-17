using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Chemical : MediaType
        {
            private Chemical(string value, string[] extensions) : base("chemical/" + value, extensions) { }

            private Chemical(string value) : this(value, null) { }

            public static readonly Chemical AnyChemical = new Chemical("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                    || (this == AnyChemical
                        && Values.Any(v =>
                            v.Matches(fileName)));
            }
        }
    }
}
