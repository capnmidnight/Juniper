namespace Juniper
{
    public partial class MediaType
    {
        public static readonly Model Model_Example = new("example");
        public static readonly Model Model_Gltf_Binary = new("gltf-binary", new[] { "glb" });
        public static readonly Model Model_Gltf_Json = new("gltf+json", new[] { "gltf" });
        public static readonly Model Model_Iges = new("iges", new[] { "igs", "iges" });
        public static readonly Model Model_Mesh = new("mesh", new[] { "msh", "mesh", "silo" });
        public static readonly Model Model_Stl = new("stl");
        public static readonly Model Model_Vendor3mf = new("3mf");
        public static readonly Model Model_VendorColladaXml = new("vnd.collada+xml", new[] { "dae" });
        public static readonly Model Model_VendorDwf = new("vnd.dwf", new[] { "dwf" });
        public static readonly Model Model_VendorFlatland3dml = new("vnd.flatland.3dml");
        public static readonly Model Model_VendorGdl = new("vnd.gdl", new[] { "gdl" });
        public static readonly Model Model_VendorGs_Gdl = new("vnd.gs-gdl");
        public static readonly Model Model_VendorGsGdl = new("vnd.gs.gdl");
        public static readonly Model Model_VendorGtw = new("vnd.gtw", new[] { "gtw" });
        public static readonly Model Model_VendorMomlXml = new("vnd.moml+xml", new[] { "xml" });
        public static readonly Model Model_VendorMts = new("vnd.mts", new[] { "mts" });
        public static readonly Model Model_VendorOpengex = new("vnd.opengex");
        public static readonly Model Model_VendorParasolidTransmitBinary = new("vnd.parasolid.transmit.binary");
        public static readonly Model Model_VendorParasolidTransmitText = new("vnd.parasolid.transmit.text");
        public static readonly Model Model_VendorRosetteAnnotated_Data_Model = new("vnd.rosette.annotated-data-model");
        public static readonly Model Model_VendorUsdzZip = new("vnd.usdz+zip", new[] { "zip" });
        public static readonly Model Model_VendorValveSourceCompiled_Map = new("vnd.valve.source.compiled-map");
        public static readonly Model Model_VendorVtu = new("vnd.vtu", new[] { "vtu" });
        public static readonly Model Model_Vrml = new("vrml", new[] { "wrl", "vrml" });
        public static readonly Model Model_X3d_Vrml = new("x3d-vrml");
        public static readonly Model Model_X3dBinary = new("x3d+binary", new[] { "x3db", "x3dbz" });
        public static readonly Model Model_X3dFastinfoset = new("x3d+fastinfoset", new[] { "fastinfoset" });
        public static readonly Model Model_X3dVrml = new("x3d+vrml", new[] { "x3dv", "x3dvz" });
        public static readonly Model Model_X3dXml = new("x3d+xml", new[] { "x3d", "x3dz" });
    }
}
