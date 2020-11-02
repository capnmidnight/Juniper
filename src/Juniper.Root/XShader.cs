using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class XShader : MediaType
        {
            private XShader(string value, string[] extensions) : base("x-shader/" + value, extensions) { }

            private XShader(string value) : this(value, null) { }

            public static readonly XShader AnyXShader = new XShader("*");

            public override bool GuessMatches(string fileName)
            {
                if (ReferenceEquals(this, AnyXShader))
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
                if (ReferenceEquals(this, AnyXShader))
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
