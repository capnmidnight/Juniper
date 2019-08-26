namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Chemical : MediaType
        {
            public Chemical(string value, string[] extensions = null) : base("chemical" + value, extensions) {}

            public static readonly Chemical X_Cdx = new Chemical("x-cdx", new string[] {"cdx"});
            public static readonly Chemical X_Cif = new Chemical("x-cif", new string[] {"cif"});
            public static readonly Chemical X_Cmdf = new Chemical("x-cmdf", new string[] {"cmdf"});
            public static readonly Chemical X_Cml = new Chemical("x-cml", new string[] {"cml"});
            public static readonly Chemical X_Csml = new Chemical("x-csml", new string[] {"csml"});
            public static readonly Chemical X_Pdb = new Chemical("x-pdb");
            public static readonly Chemical X_Xyz = new Chemical("x-xyz", new string[] {"xyz"});

            public static readonly Chemical[] Values = {
                X_Cdx,
                X_Cif,
                X_Cmdf,
                X_Cml,
                X_Csml,
                X_Pdb,
                X_Xyz,
            };
        }
    }
}
