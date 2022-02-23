import { AudioElementSource } from "juniper-audio/sources/AudioElementSource";
import { src } from "juniper-dom/attrs";
import { BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, progressSplitWeighted } from "juniper-tslib";
import { createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { solid } from "./materials";
import { PlaybackButton } from "./PlaybackButton";

const I = 1 / 3;
const J = 2 / 3;
const K = 1 / 2;
const YouTube360MonoCubeMapGeom = createQuadGeometry([
    [-K, +K, -K, I, 1],
    [+K, +K, -K, J, 1],
    [+K, -K, -K, J, K],
    [-K, -K, -K, I, K]
], [
    [+K, +K, -K, J, 1],
    [+K, +K, +K, 1, 1],
    [+K, -K, +K, 1, K],
    [+K, -K, -K, J, K]
], [
    [-K, +K, +K, 0, 1],
    [-K, +K, -K, I, 1],
    [-K, -K, -K, I, K],
    [-K, -K, +K, 0, K]
], [
    [+K, +K, +K, J, K],
    [-K, +K, +K, J, 0],
    [-K, -K, +K, I, 0],
    [+K, -K, +K, I, K]
], [
    [+K, +K, -K, 1, K],
    [-K, +K, -K, 1, 0],
    [-K, +K, +K, J, 0],
    [+K, +K, +K, J, K]
], [
    [+K, -K, +K, I, K],
    [-K, -K, +K, I, 0],
    [-K, -K, -K, 0, 0],
    [+K, -K, -K, 0, K]
]);

type YtDlpCallback = (pageUrl: string, fetcher: IFetcher, prog?: IProgress) => Promise<YTBasicResult>;

interface VideoPlayerResult {
    controls: PlaybackButton;
    videoMesh: THREE.Mesh;
}

export async function loadYouTube360MonoVideo(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog: IProgress): Promise<VideoPlayerResult> {
    const progs = progressSplitWeighted(prog, [1, 10, 100]);
    const { video, audio } = await queryYtDlp(pageUrl, env.fetcher, progs.pop());

    console.log("Getting video and audio");

    const [videoClip, audioClip] = await Promise.all([
        mediaElementReady(BackgroundVideo(false, true, false, src(video.url))),
        env.audio.createBasicClip("audio", audio.url, 1, progs.pop())
    ]);

    console.log("Got video and audio", videoClip, audioClip);

    if (audioClip instanceof AudioElementSource) {
        mediaElementForwardEvents(audioClip.input.mediaElement, videoClip);
    }

    const controls = new PlaybackButton(env, env.uiButtons, "video", null, audioClip);
    controls.object.position.set(0, 1, -0.95);

    const videoTexture = new THREE.VideoTexture(videoClip);

    const videoMesh = new THREE.Mesh(YouTube360MonoCubeMapGeom, solid({
        name: video.url,
        map: videoTexture,
        depthWrite: false
    }));
    videoMesh.scale.set(100, 100, 100);

    return { controls, videoMesh };
}