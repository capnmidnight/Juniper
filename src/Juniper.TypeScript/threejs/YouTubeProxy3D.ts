import { PlayableVideo } from "juniper-audio/sources/PlayableVideo";
import { YouTubeProxy } from "juniper-audio/YouTubeProxy";
import { IProgress } from "juniper-tslib";
import { createEACGeometry, createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { Image2DMesh } from "./Image2DMesh";
import { solid } from "./materials";
import { obj } from "./objects";
import { PlaybackButton } from "./PlaybackButton";

export type StereoFrameLayout = "left-right"
    | "right-left"
    | "top-bottom"
    | "bottom-top";

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

const YouTubeMonoEACGeom = createQuadGeometry([
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

const YouTubeStereoEACGeom_Left = createEACGeometry(1, [
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

const YouTubeStereoEAC_Right = createQuadGeometry([
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

const SquareGeom = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 1],
    [+1 / 2, +1 / 2, -1 / 2, 1, 1],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0]
]);

function linkControls(video: THREE.Object3D, controls: PlaybackButton, setScale: boolean) {
    if (setScale) {
        video.scale.set(100, 100, 100);
    }
    video.visible = false;
    controls.addEventListener("play", () => video.visible = true);
    controls.addEventListener("stop", () => video.visible = false);
}

export class YouTubeProxy3D extends YouTubeProxy {

    constructor(public readonly env: Environment,
        makeProxyURL: (path: string) => string) {
        super(env.fetcher, makeProxyURL)
    }

    private async loadVideoMaterial(pageURL: string, label: string, prog?: IProgress): Promise<VideoMaterialResult> {
        const [audioElem, videoElem, thumbnailElem] = await this.loadElements(pageURL, prog);

        const audioClip = await this.env.audio.createBasicClip(pageURL, audioElem, 1);
        const title = (label || thumbnailElem.title.substring(0, 25));
        const video = new PlayableVideo(videoElem);
        const controls = new PlaybackButton(this.env, this.env.uiButtons, pageURL, title, audioClip);
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

    async loadMonoPlane(pageURL: string, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, prog);

        const vidMesh = new THREE.Mesh(SquareGeom, material);
        vidMesh.name = "Frame-2D";
        vidMesh.scale.set(1, video.height / video.width, 1);

        const videoRig = obj("VideoContainer", vidMesh);
        linkControls(videoRig, controls, false);

        return { controls, videoRig, video, thumbnail };
    }

    async loadMonoEAC(pageURL: string, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, prog);

        const videoRig = new THREE.Mesh(YouTubeMonoEACGeom, material);
        videoRig.name = "Frame-360";

        linkControls(videoRig, controls, true);

        return { controls, videoRig, video, thumbnail };
    }

    async loadStereoEAC(pageURL: string, layout: StereoFrameLayout, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video, thumbnail } = await this.loadVideoMaterial(pageURL, label, prog);

        const names = layout.split('-');

        const vidMesh1 = new THREE.Mesh(YouTubeStereoEACGeom_Left, material);
        vidMesh1.name = "Frame-360-" + names[0];
        vidMesh1.layers.enable(0);

        const vidMesh2 = new THREE.Mesh(YouTubeStereoEAC_Right, material);
        vidMesh2.name = "Frame-360-" + names[1];
        vidMesh2.layers.disable(0);

        if (layout === "left-right") {
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

        linkControls(videoRig, controls, true);

        return { controls, videoRig, video, thumbnail };
    }
}