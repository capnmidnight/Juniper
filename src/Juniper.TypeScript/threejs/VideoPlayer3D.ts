import { IFetcher } from "juniper-fetcher";
import { BaseVideoPlayer } from "juniper-video/BaseVideoPlayer";
import { cleanup } from "./cleanup";
import { createEACGeometry, createQuadGeometry } from "./CustomGeometry";
import { Image2DMesh } from "./Image2DMesh";
import { IUpdatable } from "./IUpdatable";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { solidTransparent } from "./materials";
import { ErsatzObject, obj } from "./objects";

export type SphereEncodingName = "N/A"
    | "Cubemap"
    | "Equi-Angular Cubemap (YouTube)"
    | "Equirectangular"
    | "Half Equirectangular"
    | "Panoramic";

export const SphereEncodingNames: SphereEncodingName[] = [
    "N/A",
    "Cubemap",
    "Equi-Angular Cubemap (YouTube)",
    "Equirectangular",
    "Half Equirectangular",
    "Panoramic"
];

export type StereoLayoutName = "mono"
    | "left-right"
    | "right-left"
    | "top-bottom"
    | "bottom-top";

export const StereoLayoutNames: StereoLayoutName[] = [
    "mono",
    "left-right",
    "right-left",
    "top-bottom",
    "bottom-top"
];

export class VideoPlayer3D
    extends BaseVideoPlayer
    implements ErsatzObject, IUpdatable {

    private readonly material: THREE.MeshBasicMaterial;
    private readonly vidMesh1: Image2DMesh;
    private readonly vidMesh2: Image2DMesh;

    readonly object: THREE.Object3D;

    constructor(fetcher: IFetcher, layerMgr: IWebXRLayerManager, audioCtx: AudioContext) {
        super(audioCtx);

        this.material = solidTransparent({ name: "videoPlayer-material" });
        this.material.map = new THREE.VideoTexture(this.video);

        this.vidMesh1 = new Image2DMesh(fetcher, layerMgr, "videoPlayer-leftEye", false, this.material);
        this.vidMesh1.renderOrder = 4;
        this.vidMesh1.mesh.layers.enable(0);


        this.vidMesh2 = new Image2DMesh(fetcher, layerMgr, "videoPlayer-rightEye", false, this.material);
        this.vidMesh2.renderOrder = 4;
        this.vidMesh2.mesh.layers.disable(0);

        this.object = obj("videoPlayer",
            this.vidMesh1,
            this.vidMesh2
        );
    }

    protected override onDisposing(): void {
        super.onDisposing();
        cleanup(this.object);
    }

    update(dt: number, frame?: XRFrame) {
        this.vidMesh1.update(dt, frame);
        this.vidMesh2.update(dt, frame);
    }

    isSupported(encoding: SphereEncodingName, layout: StereoLayoutName): boolean {
        return encoding === "N/A"
            || encoding === "Equi-Angular Cubemap (YouTube)"
            && layout !== "top-bottom"
            && layout !== "bottom-top";
    }

    setStereoParameters(encoding: SphereEncodingName, layout: StereoLayoutName) {
        this.vidMesh1.mesh.layers.disable(1);
        this.vidMesh2.mesh.layers.disable(1);
        this.vidMesh1.mesh.layers.disable(2);
        this.vidMesh2.mesh.layers.disable(2);

        if (layout === "left-right"
            || layout === "top-bottom") {
            this.vidMesh1.mesh.layers.enable(2);
            this.vidMesh2.mesh.layers.enable(1);
        }
        else if (layout !== "mono") {
            this.vidMesh1.mesh.layers.enable(1);
            this.vidMesh2.mesh.layers.enable(2);
        }

        this.vidMesh1.mesh.geometry.dispose();
        this.vidMesh1.mesh.geometry = MonoPlaneGeom;
        this.vidMesh2.mesh.geometry.dispose();
        this.vidMesh2.mesh.geometry = MonoPlaneGeom;

        if (encoding === "N/A") {
            this.object.scale.set(1, 1, 1);
            this.vidMesh1.visible = true;
            this.vidMesh2.visible = layout !== "mono";

            if (layout !== "mono") {
                const names = layout.split('-');
                this.vidMesh1.mesh.geometry = StereoPlaneGeoms.get(names[0]);
                this.vidMesh2.mesh.geometry = StereoPlaneGeoms.get(names[1]);
            }

            const aspect = this.height / this.width;
            if (layout === "mono") {
                this.vidMesh1.scale.set(1, aspect, 1);
            }
            else if (layout === "left-right"
                || layout === "right-left") {
                this.vidMesh1.scale.set(1, 2 * aspect, 1);
            }
            else {
                this.vidMesh1.scale.set(1, 0.5 * aspect, 1);
            }
        }
        else if (encoding === "Equi-Angular Cubemap (YouTube)"
            && layout !== "top-bottom"
            && layout !== "bottom-top") {

            this.vidMesh1.scale.set(100, 100, 100);

            if (layout === "mono") {
                this.vidMesh1.mesh.geometry = MonoEACGeom;
            }
            else {
                const names = layout.split('-');
                this.vidMesh1.mesh.geometry = StereoEACGeoms.get(names[0]);
                this.vidMesh2.mesh.geometry = StereoEACGeoms.get(names[1]);
            }
        }
        else {
            throw new Error(`Not supported [encoding: ${encoding}, layout: ${layout}]`);
        }

        this.vidMesh2.scale.copy(this.vidMesh1.scale);
    }
}

const EACSubDivisions = 4;

const MonoEACGeom = createEACGeometry(EACSubDivisions, [
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
]);

const StereoEACGeom_Left = createEACGeometry(EACSubDivisions, [
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
]);

const StereoEACGeom_Right = createEACGeometry(EACSubDivisions, [
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
]);

const StereoEACGeoms = new Map([
    ["left", StereoEACGeom_Left],
    ["right", StereoEACGeom_Right]
]);

const MonoPlaneGeom = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.0, 1.0],
    [+1 / 2, +1 / 2, 0, 1.0, 1.0],
    [+1 / 2, -1 / 2, 0, 1.0, 0.0],
    [-1 / 2, -1 / 2, 0, 0.0, 0.0]
]);

const StereoPlaneGeom_Left = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.0, 1.0],
    [+1 / 2, +1 / 2, 0, 0.5, 1.0],
    [+1 / 2, -1 / 2, 0, 0.5, 0.0],
    [-1 / 2, -1 / 2, 0, 0.0, 0.0]
]);

const StereoPlaneGeom_Right = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.5, 1.0],
    [+1 / 2, +1 / 2, 0, 1.0, 1.0],
    [+1 / 2, -1 / 2, 0, 1.0, 0.0],
    [-1 / 2, -1 / 2, 0, 0.5, 0.0]
]);

const StereoPlaneGeom_Top = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.0, 1.0],
    [+1 / 2, +1 / 2, 0, 1.0, 1.0],
    [+1 / 2, -1 / 2, 0, 1.0, 0.5],
    [-1 / 2, -1 / 2, 0, 0.0, 0.5]
]);

const StereoPlaneGeom_Bottom = createQuadGeometry([
    [-1 / 2, +1 / 2, 0, 0.0, 0.5],
    [+1 / 2, +1 / 2, 0, 1.0, 0.5],
    [+1 / 2, -1 / 2, 0, 1.0, 0.0],
    [-1 / 2, -1 / 2, 0, 0.0, 0.0]
]);

const StereoPlaneGeoms = new Map([
    ["left", StereoPlaneGeom_Left],
    ["right", StereoPlaneGeom_Right],
    ["top", StereoPlaneGeom_Top],
    ["bottom", StereoPlaneGeom_Bottom]
]);