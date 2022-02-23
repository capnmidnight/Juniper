import { IPlayable } from "juniper-audio/sources/IPlayable";
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

interface VideoMaterialResult {
    controls: PlaybackButton;
    material: THREE.MeshBasicMaterial;
    width: number;
    height: number;
}

export interface VideoPlayerResult {
    controls: PlaybackButton;
    video: THREE.Object3D;
}

async function loadVideoMaterial(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog?: IProgress): Promise<VideoMaterialResult> {
    const progs = progressSplitWeighted(prog, [1.000, 10.000]);
    const { video, audio, title, width, height } = await queryYtDlp(pageUrl, env.fetcher, progs.pop());

    prog = progs.pop();

    if (isNullOrUndefined(video)) {
        throw new Error("No video found");
    }

    let vidClip: HTMLVideoElement;
    let controlClip: IPlayable;
    if (isDefined(audio)) {
        const [videoClip, audioClip] = await Promise.all([
            mediaElementReady(BackgroundVideo(false, true, false, src(video.url))),
            env.audio.createBasicClip("audio", audio.url, 1.000, prog)
        ]);

        mediaElementForwardEvents(audioClip.input.mediaElement, videoClip);

        vidClip = videoClip;
        controlClip = audioClip;
    }
    else {
        const videoClip = await mediaElementReady(BackgroundVideo(false, true, false, src(video.url)));
        vidClip = videoClip;
        controlClip = new PlayableVideo(vidClip);
        prog.report(1, 1, video.url);
    }

    const controls = new PlaybackButton(env, env.uiButtons, "video", title.substring(0, 25), controlClip);
    controls.object.position.set(0.000, 1.000, -0.95);

    const videoTexture = new THREE.VideoTexture(vidClip);

    const material = solid({
        name: video.url,
        map: videoTexture,
        depthWrite: false
    });
    return { controls, material, width, height };
}

function linkControls(video: THREE.Object3D, controls: PlaybackButton, setScale: boolean) {
    if (setScale) {
        video.scale.set(100, 100, 100);
    }
    video.visible = false;
    controls.addEventListener("play", () => video.visible = true);
    controls.addEventListener("stop", () => video.visible = false);
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

export async function loadYouTubeMonoEAC(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog: IProgress): Promise<VideoPlayerResult> {
    const { controls, material } = await loadVideoMaterial(env, pageUrl, queryYtDlp, prog);

    const video = new THREE.Mesh(YouTubeMonoEACGeom, material);
    video.name = "Frame-360";

    linkControls(video, controls, true);

    return { controls, video };
}



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
]
);

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

export type StereoFrameLayout = "left-right"
    | "right-left";

export async function loadYouTubeStereoEAC(env: Environment, pageUrl: string, layout: StereoFrameLayout, queryYtDlp: YtDlpCallback, prog?: IProgress): Promise<VideoPlayerResult> {
    const { controls, material } = await loadVideoMaterial(env, pageUrl, queryYtDlp, prog);

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

    const vidObject = obj("VideoContainer",
        vidMesh1,
        vidMesh2
    );

    linkControls(vidObject, controls, true);

    return { controls, video: vidObject };
}


const SquareGeom = createQuadGeometry([
    [-1 / 2, +1 / 2, -1 / 2, 0, 1],
    [+1 / 2, +1 / 2, -1 / 2, 1, 1],
    [+1 / 2, -1 / 2, -1 / 2, 1, 0],
    [-1 / 2, -1 / 2, -1 / 2, 0, 0]
]);

export async function loadYouTubeMonoPlane(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog?: IProgress): Promise<VideoPlayerResult> {
    const { controls, material, width, height } = await loadVideoMaterial(env, pageUrl, queryYtDlp, prog);

    const vidMesh = new THREE.Mesh(SquareGeom, material);
    vidMesh.name = "Frame-2D";
    vidMesh.scale.set(1, height / width, 1);

    const vidObject = obj("VideoContainer", vidMesh);
    linkControls(vidObject, controls, false);

    return { controls, video: vidObject };
}

export class YouTubeProxy {
    constructor(private readonly env: Environment, private readonly queryYtDlp: YtDlpCallback) {
    }

    loadMonoPlane(pageUrl: string, prog?: IProgress): Promise<VideoPlayerResult> {
        return loadYouTubeMonoPlane(this.env, pageUrl, this.queryYtDlp, prog);
    }

    loadMonoEAC(pageUrl: string, prog?: IProgress): Promise<VideoPlayerResult> {
        return loadYouTubeMonoEAC(this.env, pageUrl, this.queryYtDlp, prog);
    }

    loadStereoEAC(pageUrl: string, layout: StereoFrameLayout, prog?: IProgress): Promise<VideoPlayerResult> {
        return loadYouTubeStereoEAC(this.env, pageUrl, layout, this.queryYtDlp, prog);
    }
}