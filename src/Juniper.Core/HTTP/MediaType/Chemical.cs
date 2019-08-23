namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Chemical : MediaType
        {
            public Chemical(string value, string[] extensions = null) : base("chemical/" + value, extensions) {}

            public static readonly Chemical XCdx = new Chemical("x-cdx", new string[] {"cdx"});
            public static readonly Chemical XCif = new Chemical("x-cif", new string[] {"cif"});
            public static readonly Chemical XCmdf = new Chemical("x-cmdf", new string[] {"cmdf"});
            public static readonly Chemical XCml = new Chemical("x-cml", new string[] {"cml"});
            public static readonly Chemical XCsml = new Chemical("x-csml", new string[] {"csml"});
            public static readonly Chemical XPdb = new Chemical("x-pdb");
            public static readonly Chemical XXyz = new Chemical("x-xyz", new string[] {"xyz"});
        }
    }
}
