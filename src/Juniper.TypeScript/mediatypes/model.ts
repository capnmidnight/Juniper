import { specialize } from "./util";

const model = specialize("model");

export const anyModel = model("*");
export const Model_Example = model("example");
export const Model_Gltf_Binary = model("gltf-binary", "glb");
export const Model_Gltf_Json = model("gltf+json", "gltf");
export const Model_Iges = model("iges", "igs", "iges");
export const Model_Mesh = model("mesh", "msh", "mesh", "silo");
export const Model_Stl = model("stl");
export const Model_Vendor_3mf = model("3mf");
export const Model_Vendor_ColladaXml = model("vnd.collada+xml", "dae");
export const Model_Vendor_Dwf = model("vnd.dwf", "dwf");
export const Model_Vendor_Flatland3dml = model("vnd.flatland.3dml");
export const Model_Vendor_Gdl = model("vnd.gdl", "gdl");
export const Model_Vendor_Gs_Gdl = model("vnd.gs-gdl");
export const Model_Vendor_GsGdl = model("vnd.gs.gdl");
export const Model_Vendor_Gtw = model("vnd.gtw", "gtw");
export const Model_Vendor_MomlXml = model("vnd.moml+xml", "xml");
export const Model_Vendor_Mts = model("vnd.mts", "mts");
export const Model_Vendor_Opengex = model("vnd.opengex");
export const Model_Vendor_ParasolidTransmitBinary = model("vnd.parasolid.transmit.binary");
export const Model_Vendor_ParasolidTransmitText = model("vnd.parasolid.transmit.text");
export const Model_Vendor_RosetteAnnotated_Data_Model = model("vnd.rosette.annotated-data-model");
export const Model_Vendor_UsdzZip = model("vnd.usdz+zip", "zip");
export const Model_Vendor_ValveSourceCompiled_Map = model("vnd.valve.source.compiled-map");
export const Model_Vendor_Vtu = model("vnd.vtu", "vtu");
export const Model_Vrml = model("vrml", "wrl", "vrml");
export const Model_X3d_Vrml = model("x3d-vrml");
export const Model_X3dBinary = model("x3d+binary", "x3db", "x3dbz");
export const Model_X3dFastinfoset = model("x3d+fastinfoset", "fastinfoset");
export const Model_X3dVrml = model("x3d+vrml", "x3dv", "x3dvz");
export const Model_X3dXml = model("x3d+xml", "x3d", "x3dz");
export const allModel = [
    Model_Example,
    Model_Gltf_Binary,
    Model_Gltf_Json,
    Model_Iges,
    Model_Mesh,
    Model_Stl,
    Model_Vendor_3mf,
    Model_Vendor_ColladaXml,
    Model_Vendor_Dwf,
    Model_Vendor_Flatland3dml,
    Model_Vendor_Gdl,
    Model_Vendor_Gs_Gdl,
    Model_Vendor_GsGdl,
    Model_Vendor_Gtw,
    Model_Vendor_MomlXml,
    Model_Vendor_Mts,
    Model_Vendor_Opengex,
    Model_Vendor_ParasolidTransmitBinary,
    Model_Vendor_ParasolidTransmitText,
    Model_Vendor_RosetteAnnotated_Data_Model,
    Model_Vendor_UsdzZip,
    Model_Vendor_ValveSourceCompiled_Map,
    Model_Vendor_Vtu,
    Model_Vrml,
    Model_X3d_Vrml,
    Model_X3dBinary,
    Model_X3dFastinfoset,
    Model_X3dVrml,
    Model_X3dXml
];