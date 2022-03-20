import { IProgress, progressSplit } from "juniper-tslib";
import { VideoPlayer } from "juniper-video/VideoPlayer";
import { YouTubeProxy } from "juniper-video/YouTubeProxy";
import { createEACGeometry, createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { Image2DMesh } from "./Image2DMesh";
import { solid } from "./materials";
import { obj } from "./objects";

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

interface VideoMaterialResult {
    video: VideoPlayer;
    material: THREE.MeshBasicMaterial;
    thumbnail?: Image2DMesh
}

export interface VideoPlayerResult {
    video: VideoPlayer;
    videoRig: THREE.Object3D;
    videoMeshes: THREE.Mesh[];
    thumbnail?: Image2DMesh;
}

const YouTubeEACSubDivisions = 4;

const YouTubeMonoEACGeom = createEACGeometry(YouTubeEACSubDivisions, [
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

const YouTubeStereoEACGeom_Left = createEACGeometry(YouTubeEACSubDivisions, [
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

const YouTubeStereoEACGeom_Right = createEACGeometry(YouTubeEACSubDivisions, [
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

const YouTubeStereoEACGeoms = new Map([
    ["left", YouTubeStereoEACGeom_Left],
    ["right", YouTubeStereoEACGeom_Right]
]);

const MonoPlaneGeom = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 1],
    [+1 / 2, +1 / 2, -1 / 2, 1, 1],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0]
]);

const StereoPlaneGeom_Left = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 1],
    [+1 / 2, +1 / 2, -1 / 2, 0.5, 1],
    [+1 / 2, -1 / 2, -1 / 2, 0.5, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0]
]);

const StereoPlaneGeom_Right = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0.5, 1],
    [+1 / 2, +1 / 2, -1 / 2, 1, 1],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0.5, 0]
]);

const StereoPlaneGeom_Top = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 1],
    [+1 / 2, +1 / 2, -1 / 2, 1, 1],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0.5],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0.5]
]);

const StereoPlaneGeom_Bottom = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 0.5],
    [+1 / 2, +1 / 2, -1 / 2, 1, 0.5],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0]
]);

const StereoPlaneGeoms = new Map([
    ["left", StereoPlaneGeom_Left],
    ["right", StereoPlaneGeom_Right],
    ["top", StereoPlaneGeom_Top],
    ["bottom", StereoPlaneGeom_Bottom]
]);

function linkControls(video: VideoPlayer, videoRig: THREE.Object3D, thumbnail: THREE.Object3D) {
    const showVideo = (v: boolean) => {
        videoRig.visible = v;
        thumbnail.visible = !v;
    }

    video.addEventListener("played", () => showVideo(true));
    video.addEventListener("stopped", () => showVideo(false));

    showVideo(false);
}

export class YouTubeProxy3D extends YouTubeProxy {

    constructor(public readonly env: Environment,
        makeProxyURL: (path: string) => string) {
        super(env.fetcher, makeProxyURL)
    }

    private async loadVideoMaterial(pageURL: string, volume: number, prog?: IProgress): Promise<VideoMaterialResult> {
        const progs = progressSplit(prog, 2);
        const data = await this.loadData(pageURL, progs.shift());
        const video = new VideoPlayer();
        await video.load(data, progs.shift());

        this.env.audio.createBasicClip(pageURL, video.audioSource, volume);

        const videoTexture = new THREE.VideoTexture(video.video);

        const material = solid({
            name: pageURL,
            map: videoTexture,
            depthWrite: false
        });

        const thumbnail = new Image2DMesh(this.fetcher, this.env, "thumb-" + pageURL, true);
        thumbnail.mesh.setImage(video.thumbnail);
        thumbnail.mesh.objectHeight = 1 / thumbnail.mesh.imageAspectRatio;

        return {
            material,
            video,
            thumbnail
        };
    }

    async loadMonoPlane(pageURL: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { material, video, thumbnail } = await this.loadVideoMaterial(pageURL, volume, prog);

        const videoMesh = new THREE.Mesh(MonoPlaneGeom, material);
        videoMesh.name = "Frame-2D";
        videoMesh.scale.set(1, video.height / video.width, 1);

        const videoRig = obj("VideoContainer", videoMesh);
        linkControls(video, videoRig, thumbnail);

        return { videoRig, video, thumbnail, videoMeshes: [videoMesh] };
    }

    async loadMonoEAC(pageURL: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { material, video, thumbnail } = await this.loadVideoMaterial(pageURL, volume, prog);

        const videoMesh = new THREE.Mesh(YouTubeMonoEACGeom, material);
        videoMesh.name = "Frame-360";

        const videoRig = obj("VideoContainer", videoMesh);
        videoRig.scale.set(100, 100, 100);
        linkControls(video, videoRig, thumbnail);

        return { videoRig, video, thumbnail, videoMeshes: [videoMesh] };
    }

    async loadStereoPlane(pageURL: string, layout: StereoLayoutName, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { material, video, thumbnail } = await this.loadVideoMaterial(pageURL, volume, prog);

        const names = layout.split('-');

        const vidMesh1 = new THREE.Mesh(StereoPlaneGeoms.get(names[0]), material);
        vidMesh1.name = "Frame-2D-" + names[0];
        vidMesh1.layers.enable(0);

        const vidMesh2 = new THREE.Mesh(StereoPlaneGeoms.get(names[1]), material);
        vidMesh2.name = "Frame-2D-" + names[1];
        vidMesh2.layers.disable(0);

        if (layout === "left-right"
            || layout === "right-left") {
            vidMesh1.scale.set(1, 2 * video.height / video.width, 1);
            vidMesh2.scale.set(1, 2 * video.height / video.width, 1);
        }
        else {
            vidMesh1.scale.set(1, 0.5 * video.height / video.width, 1);
            vidMesh2.scale.set(1, 0.5 * video.height / video.width, 1);
        }

        if (layout === "left-right"
            || layout === "top-bottom") {
            vidMesh1.layers.enable(2);
            vidMesh2.layers.enable(1);
        }
        else {
            vidMesh1.layers.enable(1);
            vidMesh2.layers.enable(2);
        }

        const videoRig = obj("VideoContainer",
            vidMesh1,
            vidMesh2
        );
        videoRig.scale.set(100, 100, 100);

        linkControls(video, videoRig, thumbnail);

        return { videoRig, video, thumbnail, videoMeshes: [vidMesh1, vidMesh2] };
    }

    async loadStereoEAC(pageURL: string, layout: StereoLayoutName, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { material, video, thumbnail } = await this.loadVideoMaterial(pageURL, volume, prog);

        const names = layout.split('-');

        const vidMesh1 = new THREE.Mesh(YouTubeStereoEACGeoms.get(names[1]), material);
        vidMesh1.name = "Frame-360-" + names[1];
        vidMesh1.layers.enable(0);
        vidMesh1.layers.enable(1);

        const vidMesh2 = new THREE.Mesh(YouTubeStereoEACGeoms.get(names[0]), material);
        vidMesh2.name = "Frame-360-" + names[0];
        vidMesh2.layers.disable(0);
        vidMesh2.layers.enable(2);

        const videoRig = obj("VideoContainer",
            vidMesh1,
            vidMesh2
        );
        videoRig.scale.set(100, 100, 100);

        linkControls(video, videoRig, thumbnail);

        return { videoRig, video, thumbnail, videoMeshes: [vidMesh2, vidMesh1] };
    }

    isSupported(encoding: SphereEncodingName, layout: StereoLayoutName): boolean {
        return encoding === "N/A"
            || encoding === "Equi-Angular Cubemap (YouTube)"
            && layout !== "top-bottom"
            && layout !== "bottom-top";
    }

    load3D(pageURL: string, encoding: SphereEncodingName, layout: StereoLayoutName, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        if (encoding === "N/A") {
            if (layout === "mono") {
                return this.loadMonoPlane(pageURL, volume, prog);
            }
            else {
                return this.loadStereoPlane(pageURL, layout, volume, prog);
            }
        }
        else if (encoding === "Equi-Angular Cubemap (YouTube)"
            && layout !== "top-bottom"
            && layout !== "bottom-top") {
            if (layout === "mono") {
                return this.loadMonoEAC(pageURL, volume, prog);
            }
            else {
                return this.loadStereoEAC(pageURL, layout, volume, prog);
            }
        }
        else {
            throw new Error(`Not supported [encoding: ${encoding}, layout: ${layout}]`);
        }
    }
}