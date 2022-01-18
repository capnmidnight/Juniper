namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Chemical : MediaType
        {
            public static readonly Chemical X_Cdx = new("x-cdx", new string[] { "cdx" });
            public static readonly Chemical X_Cif = new("x-cif", new string[] { "cif" });
            public static readonly Chemical X_Cmdf = new("x-cmdf", new string[] { "cmdf" });
            public static readonly Chemical X_Cml = new("x-cml", new string[] { "cml" });
            public static readonly Chemical X_Csml = new("x-csml", new string[] { "csml" });
            public static readonly Chemical X_Pdb = new("x-pdb");
            public static readonly Chemical X_Xyz = new("x-xyz", new string[] { "xyz" });

            public static new readonly Chemical[] Values = {
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
