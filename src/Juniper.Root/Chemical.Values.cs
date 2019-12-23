namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Chemical : MediaType
        {
            public static readonly Chemical X_Cdx = new Chemical("x-cdx", new string[] { "cdx" });
            public static readonly Chemical X_Cif = new Chemical("x-cif", new string[] { "cif" });
            public static readonly Chemical X_Cmdf = new Chemical("x-cmdf", new string[] { "cmdf" });
            public static readonly Chemical X_Cml = new Chemical("x-cml", new string[] { "cml" });
            public static readonly Chemical X_Csml = new Chemical("x-csml", new string[] { "csml" });
            public static readonly Chemical X_Pdb = new Chemical("x-pdb");
            public static readonly Chemical X_Xyz = new Chemical("x-xyz", new string[] { "xyz" });

            new public static readonly Chemical[] Values = {
                X_Cdx,
                X_Cif,
                X_Cmdf,
                X_Cml,
                X_Csml,
                X_Pdb,
                X_Xyz
            };
        }
    }
}
