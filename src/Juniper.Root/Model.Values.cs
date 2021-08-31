namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Model : MediaType
        {
            public static readonly Model Example = new Model("example");
            public static readonly Model Gltf_Binary = new Model("gltf-binary", new[] { "glb" });
            public static readonly Model Gltf_Json = new Model("gltf+json", new[] { "gltf" });
            public static readonly Model Iges = new Model("iges", new[] { "igs", "iges" });
            public static readonly Model Mesh = new Model("mesh", new[] { "msh", "mesh", "silo" });
            public static readonly Model Stl = new Model("stl");
            public static readonly Model Vendor3mf = new Model("3mf");
            public static readonly Model VendorColladaXml = new Model("vnd.collada+xml", new[] { "dae" });
            public static readonly Model VendorDwf = new Model("vnd.dwf", new[] { "dwf" });
            public static readonly Model VendorFlatland3dml = new Model("vnd.flatland.3dml");
            public static readonly Model VendorGdl = new Model("vnd.gdl", new[] { "gdl" });
            public static readonly Model VendorGs_Gdl = new Model("vnd.gs-gdl");
            public static readonly Model VendorGsGdl = new Model("vnd.gs.gdl");
            public static readonly Model VendorGtw = new Model("vnd.gtw", new[] { "gtw" });
            public static readonly Model VendorMomlXml = new Model("vnd.moml+xml", new[] { "xml" });
            public static readonly Model VendorMts = new Model("vnd.mts", new[] { "mts" });
            public static readonly Model VendorOpengex = new Model("vnd.opengex");
            public static readonly Model VendorParasolidTransmitBinary = new Model("vnd.parasolid.transmit.binary");
            public static readonly Model VendorParasolidTransmitText = new Model("vnd.parasolid.transmit.text");
            public static readonly Model VendorRosetteAnnotated_Data_Model = new Model("vnd.rosette.annotated-data-model");
            public static readonly Model VendorUsdzZip = new Model("vnd.usdz+zip", new[] { "zip" });
            public static readonly Model VendorValveSourceCompiled_Map = new Model("vnd.valve.source.compiled-map");
            public static readonly Model VendorVtu = new Model("vnd.vtu", new[] { "vtu" });
            public static readonly Model Vrml = new Model("vrml", new[] { "wrl", "vrml" });
            public static readonly Model X3d_Vrml = new Model("x3d-vrml");
            public static readonly Model X3dBinary = new Model("x3d+binary", new[] { "x3db", "x3dbz" });
            public static readonly Model X3dFastinfoset = new Model("x3d+fastinfoset", new[] { "fastinfoset" });
            public static readonly Model X3dVrml = new Model("x3d+vrml", new[] { "x3dv", "x3dvz" });
            public static readonly Model X3dXml = new Model("x3d+xml", new[] { "x3d", "x3dz" });

            public static new readonly Model[] Values = {
                Example,
                Gltf_Binary,
                Gltf_Json,
                Iges,
                Mesh,
                Stl,
                Vendor3mf,
                VendorColladaXml,
                VendorDwf,
                VendorFlatland3dml,
                VendorGdl,
                VendorGs_Gdl,
                VendorGsGdl,
                VendorGtw,
                VendorMomlXml,
                VendorMts,
                VendorOpengex,
                VendorParasolidTransmitBinary,
                VendorParasolidTransmitText,
                VendorRosetteAnnotated_Data_Model,
                VendorUsdzZip,
                VendorValveSourceCompiled_Map,
                VendorVtu,
                Vrml,
                X3d_Vrml,
                X3dBinary,
                X3dFastinfoset,
                X3dVrml,
                X3dXml
            };
        }
    }
}
