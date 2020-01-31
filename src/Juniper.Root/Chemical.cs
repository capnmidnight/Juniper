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
                if (ReferenceEquals(this, AnyChemical))
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
