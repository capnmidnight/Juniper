using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Text : MediaType
        {
            private static List<Text> _allText;
            private static List<Text> AllTxt => _allText ??= new();
            public static IReadOnlyCollection<Text> AllText => AllTxt;
            public static readonly Text AnyText = new("*");

            internal Text(string value, params string[] extensions) : base("text", value, extensions)
            {
                if (SubType != "*")
                {
                    AllTxt.Add(this);
                }
            }
        }
    }
}
