import { AudioElementSource } from "juniper-audio/sources/AudioElementSource";
import { src } from "juniper-dom/attrs";
import { BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, progressSplitWeighted } from "juniper-tslib";
import { createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { solid } from "./materials";
import { objGraph } from "./objects";
import { PlaybackButton } from "./PlaybackButton";

type YtDlpCallback = (pageUrl: string, fetcher: IFetcher, prog?: IProgress) => Promise<YTBasicResult>;

interface VideoPlayerResult {
    controls: PlaybackButton;
    video: THREE.Object3D;
}

const _13 = 1 / 3;
const _14 = 1 / 4;
const _12 = 1 / 2;
const _23 = 2 / 3;
const _34 = 3 / 4;

const YouTube360MonoCubeMapGeom = createQuadGeometry([
    [-_12, +_12, -_12, _13, 1],
    [+_12, +_12, -_12, _23, 1],
    [+_12, -_12, -_12, _23, _12],
    [-_12, -_12, -_12, _13, _12]
], [
    [+_12, +_12, -_12, _23, 1],
    [+_12, +_12, +_12, 1, 1],
    [+_12, -_12, +_12, 1, _12],
    [+_12, -_12, -_12, _23, _12]
], [
    [-_12, +_12, +_12, 0, 1],
    [-_12, +_12, -_12, _13, 1],
    [-_12, -_12, -_12, _13, _12],
    [-_12, -_12, +_12, 0, _12]
], [
    [+_12, +_12, +_12, _23, _12],
    [-_12, +_12, +_12, _23, 0],
    [-_12, -_12, +_12, _13, 0],
    [+_12, -_12, +_12, _13, _12]
], [
    [+_12, +_12, -_12, 1, _12],
    [-_12, +_12, -_12, 1, 0],
    [-_12, +_12, +_12, _23, 0],
    [+_12, +_12, +_12, _23, _12]
], [
    [+_12, -_12, +_12, _13, _12],
    [-_12, -_12, +_12, _13, 0],
    [-_12, -_12, -_12, 0, 0],
    [+_12, -_12, -_12, 0, _12]
]);

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

    return { controls, video: videoMesh };
}



const YouTube360StereoLeftCubeMapGeom = createQuadGeometry([
    [-_12, +_12, -_12, 0, _13],
    [+_12, +_12, -_12, 0, _23],
    [+_12, -_12, -_12, _14, _23],
    [-_12, -_12, -_12, _14, _13]
], [
    [+_12, +_12, -_12, 0, _23],
    [+_12, +_12, +_12, 0, 1],
    [+_12, -_12, +_12, _14, 1],
    [+_12, -_12, -_12, _14, _23]
], [
    [-_12, +_12, +_12, 0, 0],
    [-_12, +_12, -_12, 0, _13],
    [-_12, -_12, -_12, _14, _13],
    [-_12, -_12, +_12, _14, 0]
], [
    [+_12, +_12, +_12, _14, _23],
    [-_12, +_12, +_12, _12, _23],
    [-_12, -_12, +_12, _12, _13],
    [+_12, -_12, +_12, _14, _13]
], [
    [+_12, +_12, -_12, _14, 1],
    [-_12, +_12, -_12, _12, 1],
    [-_12, +_12, +_12, _12, _23],
    [+_12, +_12, +_12, _14, _23]
], [
    [+_12, -_12, +_12, _14, _13],
    [-_12, -_12, +_12, _12, _13],
    [-_12, -_12, -_12, _12, 0],
    [+_12, -_12, -_12, _14, 0]
]
);

const YouTube360StereoRightCubeMapGeom = createQuadGeometry([
    [-_12, +_12, -_12, _12, _13],
    [+_12, +_12, -_12, _12, _23],
    [+_12, -_12, -_12, _34, _23],
    [-_12, -_12, -_12, _34, _13]
], [
    [+_12, +_12, -_12, _12, _23],
    [+_12, +_12, +_12, _12, 1],
    [+_12, -_12, +_12, _34, 1],
    [+_12, -_12, -_12, _34, _23]
], [
    [-_12, +_12, +_12, _12, 0],
    [-_12, +_12, -_12, _12, _13],
    [-_12, -_12, -_12, _34, _13],
    [-_12, -_12, +_12, _34, 0]
], [
    [+_12, +_12, +_12, _34, _23],
    [-_12, +_12, +_12, 1, _23],
    [-_12, -_12, +_12, 1, _13],
    [+_12, -_12, +_12, _34, _13]
], [
    [+_12, +_12, -_12, _34, 1],
    [-_12, +_12, -_12, 1, 1],
    [-_12, +_12, +_12, 1, _23],
    [+_12, +_12, +_12, _34, _23]
], [
    [+_12, -_12, +_12, _34, _13],
    [-_12, -_12, +_12, 1, _13],
    [-_12, -_12, -_12, 1, 0],
    [+_12, -_12, -_12, _34, 0]
]);

export async function loadYouTube360StereoVideo(env: Environment, pageUrl: string, queryYtDlp: YtDlpCallback, prog: IProgress): Promise<VideoPlayerResult> {
    const progs = progressSplitWeighted(prog, [1, 10, 100]);
    const { video, audio } = await queryYtDlp(pageUrl, env.fetcher, progs.pop());

    console.log("Getting video and audio", video, audio);

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
    const videoMaterial = solid({
        name: video.url,
        map: videoTexture,
        depthWrite: false
    });
    const videoMeshLeft = new THREE.Mesh(YouTube360StereoLeftCubeMapGeom, videoMaterial);
    videoMeshLeft.layers.set(2);
    videoMeshLeft.layers.enable(0);
    videoMeshLeft.scale.set(100, 100, 100);

    const videoMeshRight = new THREE.Mesh(YouTube360StereoRightCubeMapGeom, videoMaterial);
    videoMeshRight.layers.set(1);
    videoMeshRight.scale.set(100, 100, 100);

    const videoObject = new THREE.Object3D();
    objGraph(videoObject,
        videoMeshLeft,
        videoMeshRight
    );
    return { controls, video: videoObject };
}