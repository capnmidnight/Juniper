using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Model : MediaType
        {
            private Model(string value, string[] extensions) : base("model/" + value, extensions) { }

            private Model(string value) : this(value, null) { }

            public static readonly Model AnyModel = new Model("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyModel))
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
                if (ReferenceEquals(this, AnyModel))
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
