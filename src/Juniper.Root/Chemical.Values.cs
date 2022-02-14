namespace Juniper
{
    public partial class MediaType
    {
        public static readonly Chemical Chemical_X_Cdx = new("x-cdx", new string[] { "cdx" });
        public static readonly Chemical Chemical_X_Cif = new("x-cif", new string[] { "cif" });
        public static readonly Chemical Chemical_X_Cmdf = new("x-cmdf", new string[] { "cmdf" });
        public static readonly Chemical Chemical_X_Cml = new("x-cml", new string[] { "cml" });
        public static readonly Chemical Chemical_X_Csml = new("x-csml", new string[] { "csml" });
        public static readonly Chemical Chemical_X_Pdb = new("x-pdb");
        public static readonly Chemical Chemical_X_Xyz = new("x-xyz", new string[] { "xyz" });
    }
}
