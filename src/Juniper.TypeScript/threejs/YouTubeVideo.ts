import { AudioElementSource } from "juniper-audio/sources/AudioElementSource";
import { BackgroundVideo, mediaElementForwardEvents } from "juniper-dom/tags";
import { createQuadGeometry } from "./CustomGeometry";
import { Environment } from "./environment/Environment";
import { solid } from "./materials";
import { objGraph } from "./objects";
import { PlaybackButton } from "./PlaybackButton";
import { IProgress, once, progressSplitWeighted } from "juniper-tslib";
import { IFetcher } from "juniper-fetcher";

const I = 1 / 3;
const J = 2 / 3;
const K = 1 / 2;
const YouTubeCubeMapGeom = createQuadGeometry([
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

export class YouTubeVideo extends THREE.Object3D {

    constructor(private readonly pageUrl: string, private readonly env: Environment, private readonly queryYtDlp: YtDlpCallback) {
        super();
    }

    private _pb: PlaybackButton = null;
    get playbackButton(): THREE.Object3D {
        if (this._pb === null) {
            return null;
        }

        return this._pb.object;
    }

    private _vid: THREE.Mesh = null;
    get video(): THREE.Mesh {
        return this._vid;
    }

    async load(prog: IProgress): Promise<void> {
        const progs = progressSplitWeighted(prog, [1, 10, 100]);
        const { video, audio } = await this.queryYtDlp(this.pageUrl, this.env.fetcher, progs.pop());

        const vid = BackgroundVideo(false, true, false);
        const videoTask = once<HTMLMediaElementEventMap, "canplay">(vid, "canplay");
        vid.src = video.url;

        console.log("Getting video and audio");

        const [audioClip, _] = await Promise.all([
            this.env.audio.createBasicClip("audio", audio.url, 1, progs.pop()),
            videoTask
        ]);

        console.log("Got video and audio", vid, audioClip);

        if (audioClip instanceof AudioElementSource) {
            mediaElementForwardEvents(audioClip.input.mediaElement, vid);
        }

        this._pb = new PlaybackButton(this.env, this.env.uiButtons, "video", null, audioClip);
        this._pb.object.position.set(0, 1, -0.95);

        const vidTex = new THREE.VideoTexture(vid);

        this._vid = new THREE.Mesh(YouTubeCubeMapGeom, solid({
            name: video.url,
            map: vidTex,
            depthWrite: false
        }));
        this._vid.scale.set(100, 100, 100);

        objGraph(this,
            this._pb,
            this._vid);
    }
}
