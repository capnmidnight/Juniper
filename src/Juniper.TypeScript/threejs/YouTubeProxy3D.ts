import { PlayableVideo } from "juniper-audio/sources/PlayableVideo";
import { MediaEventForwardingDirection, YouTubeProxy } from "juniper-audio/YouTubeProxy";
import { IProgress, isDefined } from "juniper-tslib";
import { createEACGeometry, createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { Image2DMesh } from "./Image2DMesh";
import { solid } from "./materials";
import { obj } from "./objects";
import { PlaybackButton } from "./PlaybackButton";

const fwdDir: MediaEventForwardingDirection = "video-to-audio";

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

interface BaseVideoResult {
    controls: PlaybackButton;
    video: PlayableVideo;
}

interface VideoMaterialResult extends BaseVideoResult {
    material: THREE.MeshBasicMaterial;
    thumbnail?: Image2DMesh
}

export interface VideoPlayerResult extends BaseVideoResult {
    videoRig: THREE.Object3D;
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

function linkControls(video: THREE.Object3D, thumbnail: THREE.Object3D, controls: PlaybackButton, setScale: boolean) {
    if (setScale) {
        video.scale.set(100, 100, 100);
    }
    const showVideo = (v: boolean) => {
        video.visible = v;
        thumbnail.visible = !v;
    }

    controls.addEventListener("play", () => showVideo(true));
    controls.addEventListener("stop", () => showVideo(false));

    showVideo(false);
}

export class YouTubeProxy3D extends YouTubeProxy {

    constructor(public readonly env: Environment,
        makeProxyURL: (path: string) => string) {
        super(env.fetcher, makeProxyURL)
    }

    private async loadVideoMaterial(pageURL: string, label: string, volume: number, prog?: IProgress): Promise<VideoMaterialResult> {
        const [audioElem, videoElem, thumbnailElem] = await this.loadElements(pageURL, fwdDir, prog);
        const title = (label || thumbnailElem.title.substring(0, 25));
        const video = new PlayableVideo(videoElem);

        const playable = isDefined(audioElem) && fwdDir === "audio-to-video"
            ? this.env.audio.createBasicClip(pageURL, audioElem, volume)
            : video;

        const controls = new PlaybackButton(this.env, this.env.uiButtons, pageURL, title, playable);
        controls.object.renderOrder = 5;

        const videoTexture = new THREE.VideoTexture(videoElem);

        const material = solid({
            name: pageURL,
            map: videoTexture,
            depthWrite: false
        });

        const thumbnail = new Image2DMesh(this.env, "thumb-" + pageURL, true);
        thumbnail.mesh.setImage(thumbnailElem);
        thumbnail.mesh.objectHeight = 1 / thumbnail.mesh.imageAspectRatio;

        return {
            controls,
            material,
            video,
            thumbnail
        };
    }

    async loadMonoPlane(pageURL: string, label?: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, volume, prog);

        const vidMesh = new THREE.Mesh(MonoPlaneGeom, material);
        vidMesh.name = "Frame-2D";
        vidMesh.scale.set(1, video.height / video.width, 1);

        const videoRig = obj("VideoContainer", vidMesh);
        linkControls(videoRig, thumbnail, controls, false);

        return { controls, videoRig, video, thumbnail };
    }

    async loadMonoEAC(pageURL: string, label?: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, volume, prog);

        const videoRig = new THREE.Mesh(YouTubeMonoEACGeom, material);
        videoRig.name = "Frame-360";

        linkControls(videoRig, thumbnail, controls, true);

        return { controls, videoRig, video, thumbnail };
    }

    async loadStereoPlane(pageURL: string, layout: StereoLayoutName, label?: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, volume, prog);

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

        linkControls(videoRig, thumbnail, controls, true);

        return { controls, videoRig, video, thumbnail };
    }

    async loadStereoEAC(pageURL: string, layout: StereoLayoutName, label?: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, volume, prog);

        const names = layout.split('-');

        const vidMesh1 = new THREE.Mesh(YouTubeStereoEACGeoms.get(names[0]), material);
        vidMesh1.name = "Frame-360-" + names[0];
        vidMesh1.layers.enable(0);

        const vidMesh2 = new THREE.Mesh(YouTubeStereoEACGeoms.get(names[1]), material);
        vidMesh2.name = "Frame-360-" + names[1];
        vidMesh2.layers.disable(0);

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

        linkControls(videoRig, thumbnail, controls, true);

        return { controls, videoRig, video, thumbnail };
    }

    isSupported(encoding: SphereEncodingName, layout: StereoLayoutName): boolean {
        return encoding === "N/A"
            || encoding === "Equi-Angular Cubemap (YouTube)"
                && layout !== "top-bottom"
                && layout !== "bottom-top";
    }

    load(pageURL: string, encoding: SphereEncodingName, layout: StereoLayoutName, label?: string, volume: number = 1, prog?: IProgress): Promise<VideoPlayerResult> {
        if (encoding === "N/A") {
            if (layout === "mono") {
                return this.loadMonoPlane(pageURL, label, volume, prog);
            }
            else {
                return this.loadStereoPlane(pageURL, layout, label, volume, prog);
            }
        }
        else if (encoding === "Equi-Angular Cubemap (YouTube)"
            && layout !== "top-bottom"
            && layout !== "bottom-top") {
            if (layout === "mono") {
                return this.loadMonoEAC(pageURL, label, volume, prog);
            }
            else {
                return this.loadStereoEAC(pageURL, layout, label, volume, prog);
            }
        }
        else {
            throw new Error(`Not supported [encoding: ${encoding}, layout: ${layout}]`);
        }
    }
}