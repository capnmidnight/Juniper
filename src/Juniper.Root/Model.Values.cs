namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Model : MediaType
        {
            public static readonly Model Example = new("example");
            public static readonly Model Gltf_Binary = new("gltf-binary", new[] { "glb" });
            public static readonly Model Gltf_Json = new("gltf+json", new[] { "gltf" });
            public static readonly Model Iges = new("iges", new[] { "igs", "iges" });
            public static readonly Model Mesh = new("mesh", new[] { "msh", "mesh", "silo" });
            public static readonly Model Stl = new("stl");
            public static readonly Model Vendor3mf = new("3mf");
            public static readonly Model VendorColladaXml = new("vnd.collada+xml", new[] { "dae" });
            public static readonly Model VendorDwf = new("vnd.dwf", new[] { "dwf" });
            public static readonly Model VendorFlatland3dml = new("vnd.flatland.3dml");
            public static readonly Model VendorGdl = new("vnd.gdl", new[] { "gdl" });
            public static readonly Model VendorGs_Gdl = new("vnd.gs-gdl");
            public static readonly Model VendorGsGdl = new("vnd.gs.gdl");
            public static readonly Model VendorGtw = new("vnd.gtw", new[] { "gtw" });
            public static readonly Model VendorMomlXml = new("vnd.moml+xml", new[] { "xml" });
            public static readonly Model VendorMts = new("vnd.mts", new[] { "mts" });
            public static readonly Model VendorOpengex = new("vnd.opengex");
            public static readonly Model VendorParasolidTransmitBinary = new("vnd.parasolid.transmit.binary");
            public static readonly Model VendorParasolidTransmitText = new("vnd.parasolid.transmit.text");
            public static readonly Model VendorRosetteAnnotated_Data_Model = new("vnd.rosette.annotated-data-model");
            public static readonly Model VendorUsdzZip = new("vnd.usdz+zip", new[] { "zip" });
            public static readonly Model VendorValveSourceCompiled_Map = new("vnd.valve.source.compiled-map");
            public static readonly Model VendorVtu = new("vnd.vtu", new[] { "vtu" });
            public static readonly Model Vrml = new("vrml", new[] { "wrl", "vrml" });
            public static readonly Model X3d_Vrml = new("x3d-vrml");
            public static readonly Model X3dBinary = new("x3d+binary", new[] { "x3db", "x3dbz" });
            public static readonly Model X3dFastinfoset = new("x3d+fastinfoset", new[] { "fastinfoset" });
            public static readonly Model X3dVrml = new("x3d+vrml", new[] { "x3dv", "x3dvz" });
            public static readonly Model X3dXml = new("x3d+xml", new[] { "x3d", "x3dz" });

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
