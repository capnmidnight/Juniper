using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class XShader : MediaType
        {
            private static List<XShader> _allXShad;
            private static List<XShader> AllXShad => _allXShad ??= new();
            public static IReadOnlyCollection<XShader> AllXShader => AllXShad;
            public static readonly XShader AnyXShader = new("*");

            internal XShader(string value, params string[] extensions) : base("x-shader", value, extensions)
            {
                if (SubType != "*")
                {
                    AllXShad.Add(this);
                }
            }
        }
    }
}
