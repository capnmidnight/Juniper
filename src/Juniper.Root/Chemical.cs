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

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyChemical))
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
                if (ReferenceEquals(this, AnyChemical))
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
