import { AudioElementSource } from "juniper-audio/sources/AudioElementSource";
import { IPlayable } from "juniper-audio/sources/IPlayable";
import { PlayableVideo } from "juniper-audio/sources/PlayableVideo";
import { src } from "juniper-dom/attrs";
import { BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, isDefined, isNullOrUndefined, progressSplitWeighted } from "juniper-tslib";
import { createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { solid } from "./materials";
import { objGraph } from "./objects";
import { PlaybackButton } from "./PlaybackButton";

type YtDlpCallback = (pageUrl: string, fetcher: IFetcher, prog?: IProgress) => Promise<YTBasicResult>;

interface VideoMaterialResult {
    controls: PlaybackButton;
    material: THREE.MeshBasicMaterial;
}

interface VideoPlayerResult {
    controls: PlaybackButton;
    video: THREE.Object3D;
}

async function loadVideoMaterial(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog?: IProgress): Promise<VideoMaterialResult> {
    const progs = progressSplitWeighted(prog, [1.000, 10.000, 100.000]);
    const { video, audio, title } = await queryYtDlp(pageUrl, env.fetcher, progs.pop());

    if (isNullOrUndefined(video)) {
        throw new Error("No video found");
    }

    let vidClip: HTMLVideoElement;
    let controlClip: IPlayable;
    if (isDefined(audio)) {
        const [videoClip, audioClip] = await Promise.all([
            mediaElementReady(BackgroundVideo(false, true, false, src(video.url))),
            env.audio.createBasicClip("audio", audio.url, 1.000, progs.pop())
        ]);

        if (audioClip instanceof AudioElementSource) {
            mediaElementForwardEvents(audioClip.input.mediaElement, videoClip);
        }

        vidClip = videoClip;
        controlClip = audioClip;
    }
    else {
        const videoClip = await mediaElementReady(BackgroundVideo(false, true, false, src(video.url)));
        vidClip = videoClip;
        controlClip = new PlayableVideo(vidClip);
    }

    const controls = new PlaybackButton(env, env.uiButtons, "video", title.substring(0, 25), controlClip);
    controls.object.position.set(0.000, 1.000, -0.95);

    const videoTexture = new THREE.VideoTexture(vidClip);

    const material = solid({
        name: video.url,
        map: videoTexture,
        depthWrite: false
    });
    return { controls, material };
}

function linkControls(video: THREE.Object3D, controls: PlaybackButton) {
    video.scale.set(100.000, 100.000, 100);
    video.visible = false;
    controls.addEventListener("play", () => video.visible = true);
    controls.addEventListener("stop", () => video.visible = false);
}

const YouTube360MonoCubeMapGeom = createQuadGeometry([
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

export async function loadYouTube360MonoVideo(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog: IProgress): Promise<VideoPlayerResult> {
    const { controls, material } = await loadVideoMaterial(env, pageUrl, queryYtDlp, prog);

    const video = new THREE.Mesh(YouTube360MonoCubeMapGeom, material);

    linkControls(video, controls);

    return { controls, video };
}



const YouTube360StereoLeftCubeMapGeom = createQuadGeometry([
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

const YouTube360StereoRightCubeMapGeom = createQuadGeometry([
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

export async function loadYouTube360StereoVideo(env: Environment, pageUrl: string, layout: StereoFrameLayout, queryYtDlp: YtDlpCallback, prog?: IProgress): Promise<VideoPlayerResult> {
    const { controls, material } = await loadVideoMaterial(env, pageUrl, queryYtDlp, prog);

    const videoMeshLeft = new THREE.Mesh(YouTube360StereoLeftCubeMapGeom, material);
    videoMeshLeft.layers.enable(0);

    const videoMeshRight = new THREE.Mesh(YouTube360StereoRightCubeMapGeom, material);
    videoMeshRight.layers.disable(0);

    if (layout === "left-right") {
        videoMeshRight.layers.enable(1);
        videoMeshLeft.layers.enable(2);
    }
    else {
        videoMeshLeft.layers.enable(1);
        videoMeshRight.layers.enable(2);
    }

    const video = new THREE.Object3D();
    objGraph(video,
        videoMeshLeft,
        videoMeshRight
    );

    linkControls(video, controls);

    return { controls, video };
}
