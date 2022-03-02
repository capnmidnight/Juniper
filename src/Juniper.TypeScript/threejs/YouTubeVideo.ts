import { PlayableVideo } from "juniper-audio/sources/PlayableVideo";
import { src } from "juniper-dom/attrs";
import { BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, isDefined, isNullOrUndefined, progressSplitWeighted } from "juniper-tslib";
import { createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { solid } from "./materials";
import { obj } from "./objects";
import { PlaybackButton } from "./PlaybackButton";

type YtDlpCallback = (pageUrl: string, fetcher: IFetcher, prog?: IProgress) => Promise<YTBasicResult>;

export type StereoFrameLayout = "left-right"
    | "right-left";

interface BaseVideoResult {
    controls: PlaybackButton;
    video: PlayableVideo;
}

interface VideoMaterialResult extends BaseVideoResult {
    material: THREE.MeshBasicMaterial;
    width: number;
    height: number;
}

export interface VideoPlayerResult extends BaseVideoResult {
    videoRig: THREE.Object3D;
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

const YouTubeStereoEACGeom_Left = createQuadGeometry([
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

export class YouTubeProxy {
    constructor(public readonly env: Environment, private readonly queryYtDlp: YtDlpCallback) {
    }

    private loadVideoElement(vidLoc: YTMediaEntry): Promise<HTMLVideoElement> {
        return mediaElementReady(BackgroundVideo(false, true, false, src(vidLoc.url)));
    }

    private async loadMediaElements(audLoc: YTMediaEntry, vidLoc: YTMediaEntry, pageUrl: string, prog?: IProgress): Promise<HTMLVideoElement> {
        if (isDefined(audLoc)) {
            const [videoClip, audioClip] = await Promise.all([
                this.loadVideoElement(vidLoc),
                this.env.audio.createBasicClip(pageUrl, audLoc.url, 1.000, prog)
            ]);

            mediaElementForwardEvents(videoClip, audioClip.input.mediaElement);

            return videoClip;
        }
        else {
            const videoClip = await this.loadVideoElement(vidLoc);
            prog.report(1, 1, vidLoc.url);

            return videoClip;
        }
    }

    private async loadVideoMaterial(pageUrl: string, label: string, prog?: IProgress): Promise<VideoMaterialResult> {
        const progs = progressSplitWeighted(prog, [1.000, 10.000]);
        const { video: vidLoc, audio: audLoc, title, width, height } = await this.queryYtDlp(pageUrl, this.env.fetcher, progs.shift());

        prog = progs.shift();

        if (isNullOrUndefined(vidLoc)) {
            throw new Error("No video found");
        }

        const videoElem = await this.loadMediaElements(audLoc, vidLoc, pageUrl, prog);
        const video = new PlayableVideo(videoElem);
        const controls = new PlaybackButton(this.env, this.env.uiButtons, pageUrl, (label || title.substring(0, 25)), video);
        const videoTexture = new THREE.VideoTexture(videoElem);
        const material = solid({
            name: pageUrl,
            map: videoTexture,
            depthWrite: false
        });
        return { controls, material, width, height, video };
    }

    async loadMonoPlane(pageUrl: string, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, width, height, video } = await this.loadVideoMaterial(pageUrl, label, prog);

        const vidMesh = new THREE.Mesh(SquareGeom, material);
        vidMesh.name = "Frame-2D";
        vidMesh.scale.set(1, height / width, 1);

        const videoRig = obj("VideoContainer", vidMesh);
        linkControls(videoRig, controls, false);

        return { controls, videoRig, video };
    }

    async loadMonoEAC(pageUrl: string, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video } = await this.loadVideoMaterial(pageUrl, label, prog);

        const videoRig = new THREE.Mesh(YouTubeMonoEACGeom, material);
        videoRig.name = "Frame-360";

        linkControls(videoRig, controls, true);

        return { controls, videoRig, video };
    }

    async loadStereoEAC(pageUrl: string, layout: StereoFrameLayout, label?: string, prog?: IProgress): Promise<VideoPlayerResult> {
        const { controls, material, video } = await this.loadVideoMaterial(pageUrl, label, prog);

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

        return { controls, videoRig, video };
    }
}