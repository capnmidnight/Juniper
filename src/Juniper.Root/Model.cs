using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Model : MediaType
        {
            private Model(string value, string[] extensions) : base("model/" + value, extensions) {}

            private Model(string value) : this(value, null) {}

            public static readonly Model AnyModel = new Model("*");

            public override bool Matches(string fileName)
            {
                return base.Matches(fileName)
                        || this == AnyModel
                            && Values.Any(v => v.Matches(fileName));
            }
        }
    }
}
