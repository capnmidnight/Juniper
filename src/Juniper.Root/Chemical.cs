using System.Collections.Generic;

namespace Juniper
{
    public partial class MediaType
    {
        public partial class Chemical : MediaType
        {
            private static List<Chemical> _allChem;
            private static List<Chemical> AllChem => _allChem ??= new();
            public static IReadOnlyCollection<Chemical> AllChemical => AllChem;
            public static readonly Chemical AnyChemical = new("*");

            public Chemical(string value, params string[] extensions) : base("chemical", value, extensions)
            {
                if (SubType != "*")
                {
                    AllChem.Add(this);
                }
            }
        }
    }
}
