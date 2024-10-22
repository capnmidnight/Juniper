namespace Juniper;

public partial class MediaType
{
    public static readonly Model Model_Example = new("example");
    public static readonly Model Model_Gltf_Binary = new("gltf-binary", "glb");
    public static readonly Model Model_Gltf_Json = new("gltf+json", "gltf");
    public static readonly Model Model_Iges = new("iges", "igs", "iges");
    public static readonly Model Model_Mesh = new("mesh", "msh", "mesh", "silo");
    public static readonly Model Model_Stl = new("stl");
    public static readonly Model Model_Vendor_3mf = new("3mf");
    public static readonly Model Model_Vendor_ColladaXml = new("vnd.collada+xml", "dae");
    public static readonly Model Model_Vendor_Dwf = new("vnd.dwf", "dwf");
    public static readonly Model Model_Vendor_Flatland3dml = new("vnd.flatland.3dml");
    public static readonly Model Model_Vendor_Gdl = new("vnd.gdl", "gdl");
    public static readonly Model Model_Vendor_Gs_Gdl = new("vnd.gs-gdl");
    public static readonly Model Model_Vendor_GsGdl = new("vnd.gs.gdl");
    public static readonly Model Model_Vendor_Gtw = new("vnd.gtw", "gtw");
    public static readonly Model Model_Vendor_MomlXml = new("vnd.moml+xml", "xml");
    public static readonly Model Model_Vendor_Mts = new("vnd.mts", "mts");
    public static readonly Model Model_Vendor_Opengex = new("vnd.opengex");
    public static readonly Model Model_Vendor_ParasolidTransmitBinary = new("vnd.parasolid.transmit.binary");
    public static readonly Model Model_Vendor_ParasolidTransmitText = new("vnd.parasolid.transmit.text");
    public static readonly Model Model_Vendor_RosetteAnnotated_Data_Model = new("vnd.rosette.annotated-data-model");
    public static readonly Model Model_Vendor_UsdzZip = new("vnd.usdz+zip", "zip");
    public static readonly Model Model_Vendor_ValveSourceCompiled_Map = new("vnd.valve.source.compiled-map");
    public static readonly Model Model_Vendor_Vtu = new("vnd.vtu", "vtu");
    public static readonly Model Model_Vrml = new("vrml", "wrl", "vrml");
    public static readonly Model Model_X3d_Vrml = new("x3d-vrml");
    public static readonly Model Model_X3dBinary = new("x3d+binary", "x3db", "x3dbz");
    public static readonly Model Model_X3dFastinfoset = new("x3d+fastinfoset", "fastinfoset");
    public static readonly Model Model_X3dVrml = new("x3d+vrml", "x3dv", "x3dvz");
    public static readonly Model Model_X3dXml = new("x3d+xml", "x3d", "x3dz");
}
