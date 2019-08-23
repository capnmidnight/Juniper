namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Model : MediaType
        {
            public Model(string value, string[] extensions = null) : base("model/" + value, extensions) {}

            public static readonly Model Example = new Model("example");
            public static readonly Model GltfBinary = new Model("gltf-binary");
            public static readonly Model GltfJson = new Model("gltf+json", new string[] {"json"});
            public static readonly Model Iges = new Model("iges", new string[] {"igs", "iges"});
            public static readonly Model Mesh = new Model("mesh", new string[] {"msh", "mesh", "silo"});
            public static readonly Model Stl = new Model("stl");
            public static readonly Model Vendor3mf = new Model("3mf");
            public static readonly Model VendorColladaXml = new Model("vnd.collada+xml", new string[] {"dae"});
            public static readonly Model VendorDwf = new Model("vnd.dwf", new string[] {"dwf"});
            public static readonly Model VendorFlatland3dml = new Model("vnd.flatland.3dml");
            public static readonly Model VendorGdl = new Model("vnd.gdl", new string[] {"gdl"});
            public static readonly Model VendorGsGdl = new Model("vnd.gs-gdl");
            public static readonly Model VendorGtw = new Model("vnd.gtw", new string[] {"gtw"});
            public static readonly Model VendorMomlXml = new Model("vnd.moml+xml", new string[] {"xml"});
            public static readonly Model VendorMts = new Model("vnd.mts", new string[] {"mts"});
            public static readonly Model VendorOpengex = new Model("vnd.opengex");
            public static readonly Model VendorParasolidTransmitBinary = new Model("vnd.parasolid.transmit.binary");
            public static readonly Model VendorParasolidTransmitText = new Model("vnd.parasolid.transmit.text");
            public static readonly Model VendorRosetteAnnotatedDataModel = new Model("vnd.rosette.annotated-data-model");
            public static readonly Model VendorUsdzZip = new Model("vnd.usdz+zip", new string[] {"zip"});
            public static readonly Model VendorValveSourceCompiledMap = new Model("vnd.valve.source.compiled-map");
            public static readonly Model VendorVtu = new Model("vnd.vtu", new string[] {"vtu"});
            public static readonly Model Vrml = new Model("vrml", new string[] {"wrl", "vrml"});
            public static readonly Model X3dBinary = new Model("x3d+binary", new string[] {"x3db", "x3dbz"});
            public static readonly Model X3dFastinfoset = new Model("x3d+fastinfoset", new string[] {"fastinfoset"});
            public static readonly Model X3dVrml = new Model("x3d+vrml", new string[] {"x3dv", "x3dvz"});
            public static readonly Model X3dXml = new Model("x3d+xml", new string[] {"x3d", "x3dz"});
        }
    }
}
