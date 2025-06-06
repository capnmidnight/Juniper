import { and, arrayClear } from "@juniper-lib/util";
import { PriorityMap } from "@juniper-lib/collections";
import { BaseVideoPlayer } from "@juniper-lib/video";
import { createEACGeometry, createQuadGeometry } from "./CustomGeometry";
import { cleanup } from "./cleanup";
import { solidTransparent } from "./materials";
import { obj } from "./objects";
import { Image2D } from "./widgets/Image2D";
export const SphereEncodingNames = /*@__PURE__*/ [
    "N/A",
    "Cubemap",
    "Cubemap Strips",
    "Equi-Angular Cubemap (YouTube)",
    "Equirectangular",
    "Half Equirectangular",
    "Panoramic"
];
export const StereoLayoutNames = /*@__PURE__*/ [
    "mono",
    "left-right",
    "right-left",
    "top-bottom",
    "bottom-top"
];
export class VideoPlayer3D extends BaseVideoPlayer {
    constructor(env, context, spatializer) {
        super("video-player-3d", context, spatializer);
        this.material = solidTransparent({ name: "videoPlayer-material" });
        this.vidMeshes = [];
        for (let i = 0; i < 2; ++i) {
            const vidMesh = new Image2D(env, `videoPlayer-view${i + 1}`, "none", this.material);
            vidMesh.setTextureMap(this.video);
            vidMesh.mesh.renderOrder = 4;
            if (i > 0) {
                vidMesh.mesh.layers.disable(0);
            }
            else {
                vidMesh.mesh.layers.enable(0);
            }
            this.vidMeshes.push(vidMesh);
        }
        this.content3d = obj("videoPlayer", ...this.vidMeshes);
    }
    get meshes() {
        return this.vidMeshes.map(v => v.mesh);
    }
    onDisposing() {
        super.onDisposing();
        cleanup(this.content3d);
        arrayClear(this.vidMeshes);
    }
    isSupported(encoding, layout) {
        return layout
            .split("-")
            .map((name) => GeomPacks.has(encoding, name))
            .reduce(and, true);
    }
    setStereoParameters(encoding, layout) {
        if (!this.isSupported(encoding, layout)) {
            throw new Error(`Not supported [encoding: ${encoding}, layout: ${layout}]`);
        }
        for (let i = 0; i < this.vidMeshes.length; ++i) {
            const vidMesh = this.vidMeshes[i];
            vidMesh.webXRLayerType = "none";
            vidMesh.mesh.layers.disable(1);
            vidMesh.mesh.layers.disable(2);
            if (layout === "left-right"
                || layout === "top-bottom") {
                vidMesh.mesh.layers.enable(this.vidMeshes.length - i);
            }
            else if (layout !== "mono") {
                vidMesh.mesh.layers.enable(i + 1);
            }
        }
        const aspect = this.height / this.width;
        if (encoding !== "N/A") {
            this.vidMeshes[0].scale.setScalar(100);
        }
        else if (layout === "mono") {
            this.vidMeshes[0].scale.set(1, aspect, 1);
        }
        else if (layout === "left-right"
            || layout === "right-left") {
            this.vidMeshes[0].scale.set(1, 2 * aspect, 1);
        }
        else {
            this.vidMeshes[0].scale.set(1, 0.5 * aspect, 1);
        }
        for (let i = 1; i < this.vidMeshes.length; ++i) {
            this.vidMeshes[i].scale.copy(this.vidMeshes[0].scale);
        }
        const names = layout.split("-");
        for (let i = 0; i < names.length; ++i) {
            const name = names[i];
            const geom = GeomPacks.get(encoding, name);
            const vidMesh = this.vidMeshes[i];
            vidMesh.webXRLayerType = "dynamic";
            vidMesh.visible = true;
            if (vidMesh.mesh.geometry !== geom) {
                cleanup(vidMesh.mesh.geometry);
                vidMesh.mesh.geometry = geom;
            }
        }
    }
}
const PlaneGeom_Mono = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.0, 1.0],
    [+1 / 2, +1 / 2, 0, 1.0, 1.0],
    [+1 / 2, -1 / 2, 0, 1.0, 0.0],
    [-1 / 2, -1 / 2, 0, 0.0, 0.0]
]);
const PlaneDef_Left = [
    [-1 / 2, +1 / 2, 0, 0.0, 1.0],
    [+1 / 2, +1 / 2, 0, 0.5, 1.0],
    [+1 / 2, -1 / 2, 0, 0.5, 0.0],
    [-1 / 2, -1 / 2, 0, 0.0, 0.0]
];
const PlanDef_Right = [
    [-1 / 2, +1 / 2, 0, 0.5, 1.0],
    [+1 / 2, +1 / 2, 0, 1.0, 1.0],
    [+1 / 2, -1 / 2, 0, 1.0, 0.0],
    [-1 / 2, -1 / 2, 0, 0.5, 0.0]
];
const CubeStripDef_Mono = [[
        [-1 / 2, +1 / 2, -1 / 2, 1 / 3, 1.000],
        [+1 / 2, +1 / 2, -1 / 2, 2 / 3, 1.000],
        [+1 / 2, -1 / 2, -1 / 2, 2 / 3, 1 / 2],
        [-1 / 2, -1 / 2, -1 / 2, 1 / 3, 1 / 2]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 2 / 3, 1.000],
        [+1 / 2, +1 / 2, +1 / 2, 1.000, 1.000],
        [+1 / 2, -1 / 2, +1 / 2, 1.000, 1 / 2],
        [+1 / 2, -1 / 2, -1 / 2, 2 / 3, 1 / 2]
    ], [
        [-1 / 2, +1 / 2, +1 / 2, 0.000, 1.000],
        [-1 / 2, +1 / 2, -1 / 2, 1 / 3, 1.000],
        [-1 / 2, -1 / 2, -1 / 2, 1 / 3, 1 / 2],
        [-1 / 2, -1 / 2, +1 / 2, 0.000, 1 / 2]
    ], [
        [+1 / 2, +1 / 2, +1 / 2, 2 / 3, 1 / 2],
        [-1 / 2, +1 / 2, +1 / 2, 2 / 3, 0.000],
        [-1 / 2, -1 / 2, +1 / 2, 1 / 3, 0.000],
        [+1 / 2, -1 / 2, +1 / 2, 1 / 3, 1 / 2]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 1.000, 1 / 2],
        [-1 / 2, +1 / 2, -1 / 2, 1.000, 0.000],
        [-1 / 2, +1 / 2, +1 / 2, 2 / 3, 0.000],
        [+1 / 2, +1 / 2, +1 / 2, 2 / 3, 1 / 2]
    ], [
        [+1 / 2, -1 / 2, +1 / 2, 1 / 3, 1 / 2],
        [-1 / 2, -1 / 2, +1 / 2, 1 / 3, 0.000],
        [-1 / 2, -1 / 2, -1 / 2, 0.000, 0.000],
        [+1 / 2, -1 / 2, -1 / 2, 0.000, 1 / 2]
    ]];
const CubeStripDef_Left = [[
        [-1 / 2, +1 / 2, -1 / 2, 0.000, 1 / 3],
        [+1 / 2, +1 / 2, -1 / 2, 0.000, 2 / 3],
        [+1 / 2, -1 / 2, -1 / 2, 1 / 4, 2 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 1 / 4, 1 / 3]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 0.000, 2 / 3],
        [+1 / 2, +1 / 2, +1 / 2, 0.000, 1.000],
        [+1 / 2, -1 / 2, +1 / 2, 1 / 4, 1.000],
        [+1 / 2, -1 / 2, -1 / 2, 1 / 4, 2 / 3]
    ], [
        [-1 / 2, +1 / 2, +1 / 2, 0.000, 0.000],
        [-1 / 2, +1 / 2, -1 / 2, 0.000, 1 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 1 / 4, 1 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 1 / 4, 0.000]
    ], [
        [+1 / 2, +1 / 2, +1 / 2, 1 / 4, 2 / 3],
        [-1 / 2, +1 / 2, +1 / 2, 1 / 2, 2 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 1 / 2, 1 / 3],
        [+1 / 2, -1 / 2, +1 / 2, 1 / 4, 1 / 3]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 1 / 4, 1.000],
        [-1 / 2, +1 / 2, -1 / 2, 1 / 2, 1.000],
        [-1 / 2, +1 / 2, +1 / 2, 1 / 2, 2 / 3],
        [+1 / 2, +1 / 2, +1 / 2, 1 / 4, 2 / 3]
    ], [
        [+1 / 2, -1 / 2, +1 / 2, 1 / 4, 1 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 1 / 2, 1 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 1 / 2, 0.000],
        [+1 / 2, -1 / 2, -1 / 2, 1 / 4, 0.000]
    ]];
const CubeStripDef_Right = [[
        [-1 / 2, +1 / 2, -1 / 2, 1 / 2, 1 / 3],
        [+1 / 2, +1 / 2, -1 / 2, 1 / 2, 2 / 3],
        [+1 / 2, -1 / 2, -1 / 2, 3 / 4, 2 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 3 / 4, 1 / 3]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 1 / 2, 2 / 3],
        [+1 / 2, +1 / 2, +1 / 2, 1 / 2, 1.000],
        [+1 / 2, -1 / 2, +1 / 2, 3 / 4, 1.000],
        [+1 / 2, -1 / 2, -1 / 2, 3 / 4, 2 / 3]
    ], [
        [-1 / 2, +1 / 2, +1 / 2, 1 / 2, 0.000],
        [-1 / 2, +1 / 2, -1 / 2, 1 / 2, 1 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 3 / 4, 1 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 3 / 4, 0.000]
    ], [
        [+1 / 2, +1 / 2, +1 / 2, 3 / 4, 2 / 3],
        [-1 / 2, +1 / 2, +1 / 2, 1.000, 2 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 1.000, 1 / 3],
        [+1 / 2, -1 / 2, +1 / 2, 3 / 4, 1 / 3]
    ], [
        [+1 / 2, +1 / 2, -1 / 2, 3 / 4, 1.000],
        [-1 / 2, +1 / 2, -1 / 2, 1.000, 1.000],
        [-1 / 2, +1 / 2, +1 / 2, 1.000, 2 / 3],
        [+1 / 2, +1 / 2, +1 / 2, 3 / 4, 2 / 3]
    ], [
        [+1 / 2, -1 / 2, +1 / 2, 3 / 4, 1 / 3],
        [-1 / 2, -1 / 2, +1 / 2, 1.000, 1 / 3],
        [-1 / 2, -1 / 2, -1 / 2, 1.000, 0.000],
        [+1 / 2, -1 / 2, -1 / 2, 3 / 4, 0.000]
    ]];
const PlaneGeom_Left = createQuadGeometry(PlaneDef_Left);
const PlaneGeom_Right = createQuadGeometry(PlanDef_Right);
const PlaneGeom_Top = createQuadGeometry(rotQuad(PlaneDef_Left));
const PlaneGeom_Bottom = createQuadGeometry(rotQuad(PlanDef_Right));
const CubeStripDef_Top = rot(CubeStripDef_Left);
const CubeStripDef_Bottom = rot(CubeStripDef_Right);
const CubeStripGeom_Mono = createQuadGeometry(...CubeStripDef_Mono);
const CubeStripGeom_Left = createQuadGeometry(...CubeStripDef_Left);
const CubeStripGeom_Right = createQuadGeometry(...CubeStripDef_Right);
const CubeStripGeom_Top = createQuadGeometry(...CubeStripDef_Top);
const CubeStripGeom_Bottom = createQuadGeometry(...CubeStripDef_Bottom);
const EACSubDivisions = 4;
const EACGeom_Mono = createEACGeometry(EACSubDivisions, ...CubeStripDef_Mono);
const EACGeom_Left = createEACGeometry(EACSubDivisions, ...CubeStripDef_Left);
const EACGeom_Right = createEACGeometry(EACSubDivisions, ...CubeStripDef_Right);
const EACGeom_Top = createEACGeometry(EACSubDivisions, ...CubeStripDef_Top);
const EACGeom_Bottom = createEACGeometry(EACSubDivisions, ...CubeStripDef_Bottom);
const GeomPacks = new PriorityMap([
    ["N/A", "mono", PlaneGeom_Mono],
    ["N/A", "left", PlaneGeom_Left],
    ["N/A", "right", PlaneGeom_Right],
    ["N/A", "top", PlaneGeom_Top],
    ["N/A", "bottom", PlaneGeom_Bottom],
    ["Cubemap Strips", "mono", CubeStripGeom_Mono],
    ["Cubemap Strips", "left", CubeStripGeom_Left],
    ["Cubemap Strips", "right", CubeStripGeom_Right],
    ["Cubemap Strips", "top", CubeStripGeom_Top],
    ["Cubemap Strips", "bottom", CubeStripGeom_Bottom],
    ["Equi-Angular Cubemap (YouTube)", "mono", EACGeom_Mono],
    ["Equi-Angular Cubemap (YouTube)", "left", EACGeom_Left],
    ["Equi-Angular Cubemap (YouTube)", "right", EACGeom_Right],
    ["Equi-Angular Cubemap (YouTube)", "top", EACGeom_Top],
    ["Equi-Angular Cubemap (YouTube)", "bottom", EACGeom_Bottom]
]);
function rotVert(vert) {
    return [vert[0], vert[1], vert[2], vert[4], 1 - vert[3]];
}
function rotQuad(quad) {
    return [
        rotVert(quad[0]),
        rotVert(quad[1]),
        rotVert(quad[2]),
        rotVert(quad[3])
    ];
}
function rot(def) {
    return def.map(rotQuad);
}
//# sourceMappingURL=VideoPlayer3D.js.map