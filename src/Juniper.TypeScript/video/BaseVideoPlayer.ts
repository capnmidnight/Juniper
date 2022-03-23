import { AudioRecord } from "juniper-audio/data";
import { Gain, MediaElementSource, removeVertex } from "juniper-audio/nodes";
import { BaseAudioSource } from "juniper-audio/sources/BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "juniper-audio/sources/IPlayable";
import { IPlayer, MediaPlayerEvents, MediaPlayerLoadingEvent } from "juniper-audio/sources/IPlayer";
import { PlaybackState } from "juniper-audio/sources/PlaybackState";
import { NoSpatializationNode } from "juniper-audio/sources/spatializers/NoSpatializationNode";
import { autoPlay, controls, loop, playsInline } from "juniper-dom/attrs";
import { Audio, ElementChild, Video } from "juniper-dom/tags";
import { Video_Vendor_Mpeg_Dash_Mpd } from "juniper-mediatypes/video";
import { arraySortByKeyInPlace, AsyncCallback, IDisposable, IProgress, isDefined, isNullOrUndefined, once, PriorityList, progressTasks, success } from "juniper-tslib";
import { FullVideoRecord, isVideoRecord } from "./data";

export abstract class BaseVideoPlayer
    extends BaseAudioSource<GainNode, MediaPlayerEvents>
    implements IPlayer, IDisposable {

    private readonly loadingEvt: MediaPlayerLoadingEvent;
    private readonly loadEvt: MediaElementSourceLoadedEvent<IPlayer>;
    private readonly playEvt: MediaElementSourcePlayedEvent<IPlayer>;
    private readonly pauseEvt: MediaElementSourcePausedEvent<IPlayer>;
    private readonly stopEvt: MediaElementSourceStoppedEvent<IPlayer>;
    private readonly progEvt: MediaElementSourceProgressEvent<IPlayer>;

    private readonly onPlay: () => void;
    private readonly onSeeked: () => void;
    private readonly onCanPlay: () => void;
    private readonly onWaiting: () => void;
    private readonly onPause: (evt: Event) => void;
    private readonly onTimeUpdate: () => Promise<void> = null;

    private videoHadAudio: boolean = false;

    private _data: FullVideoRecord = null;
    get data(): FullVideoRecord {
        return this._data;
    }

    private _loaded = false;
    public get loaded() {
        return this._loaded;
    }

    protected readonly video: HTMLVideoElement;
    protected readonly audio: HTMLAudioElement;
    private readonly videoSource: MediaElementAudioSourceNode;
    private readonly audioSource: MediaElementAudioSourceNode;

    get title() {
        return this.video.title;
    }

    protected setTitle(v: string): void {
        this.video.title = v;
        this.audio.title = v;
    }

    private readonly onError = new Map<HTMLMediaElement, AsyncCallback>();
    private readonly sourcesByURL = new Map<string, AudioRecord>();
    private readonly sources = new PriorityList<HTMLMediaElement, AudioRecord>();
    private readonly potato = new PriorityList<HTMLMediaElement, AudioRecord>();

    constructor(audioCtx: AudioContext) {
        super("JuniperVideoPlayer", audioCtx, NoSpatializationNode.instance(audioCtx));

        this.video = this.createMediaElement(Video, controls(true));
        this.audio = this.createMediaElement(Audio, controls(false));

        this.input = Gain("JuniperVideoPlayer-combiner", audioCtx);

        this.videoSource = MediaElementSource(
            "JuniperVideoPlayer-VideoNode",
            audioCtx,
            this.video,
            this.input);

        this.audioSource = MediaElementSource(
            "JuniperVideoPlayer-AudioNode",
            audioCtx,
            this.audio,
            this.input);

        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.onSeeked = () => {
            if (!this.videoHasAudio) {
                this.audio.currentTime = this.video.currentTime;
            }
        };

        this.onPlay = async () => {
            this.connect();

            this.onSeeked();
            if (!this.videoHasAudio) {
                await this.audio.play();
            }
            this.dispatchEvent(this.playEvt)
        };

        this.onPause = (evt: Event) => {
            this.disconnect();

            if (!this.videoHasAudio) {
                this.onSeeked();
                this.audio.pause();
            }
            if (this.video.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };

        let wasWaiting = false;
        this.onWaiting = () => {
            if (!this.videoHasAudio) {
                wasWaiting = true;
                this.audio.pause();
            }
        };

        this.onCanPlay = async () => {
            if (!this.videoHasAudio && wasWaiting) {
                await this.audio.play();
                wasWaiting = false;
            }
        };

        this.videoHadAudio = false;
        this.onTimeUpdate = async () => {
            const quality = this.video.getVideoPlaybackQuality();
            if (quality.totalVideoFrames === 0) {
                const onError = this.onError.get(this.video);
                if (isDefined(onError)) {
                    await onError();
                }
            }
            else if (!this.videoHasAudio) {
                const delta = this.video.currentTime - this.audio.currentTime;
                if (Math.abs(delta) > 0.25) {
                    this.audio.currentTime = this.video.currentTime;
                }
            }
            else if (!this.videoHadAudio) {
                this.videoHadAudio = true;
                this.audio.pause();
            }
            this.progEvt.value = this.video.currentTime;
            this.progEvt.total = this.video.duration;
            this.dispatchEvent(this.progEvt);
        };

        this.video.addEventListener("seeked", this.onSeeked);
        this.video.addEventListener("play", this.onPlay);
        this.video.addEventListener("pause", this.onPause);
        this.video.addEventListener("ended", this.onPause);
        this.video.addEventListener("waiting", this.onWaiting);
        this.video.addEventListener("canplay", this.onCanPlay);
        this.video.addEventListener("timeupdate", this.onTimeUpdate);


        Object.assign(window, { videoPlayer: this });
    }

    private elementHasAudio(elem: HTMLMediaElement): boolean {
        const source = this.sourcesByURL.get(elem.src);
        return isDefined(source) && source.acodec !== "none"
            || isDefined(elem.audioTracks) && elem.audioTracks.length > 0
            || isDefined(elem.webkitAudioDecodedByteCount) && elem.webkitAudioDecodedByteCount > 0
            || isDefined(elem.mozHasAudio) && elem.mozHasAudio;
    }

    private get videoHasAudio(): boolean {
        return this.elementHasAudio(this.video);
    }

    private get audioHasAudio(): boolean {
        return this.elementHasAudio(this.audio);
    }

    get hasAudio() {
        return this.audioHasAudio
            || this.videoHasAudio;
    }

    get audibleElement(): HTMLMediaElement {
        return this.videoHasAudio
            ? this.video
            : this.audio;
    }

    protected override onDisposing(): void {
        super.onDisposing();
        this.clear();
        removeVertex(this.input);
        removeVertex(this.videoSource);
        removeVertex(this.audioSource);

        this.video.removeEventListener("seeked", this.onSeeked);
        this.video.removeEventListener("play", this.onPlay);
        this.video.removeEventListener("pause", this.onPause);
        this.video.removeEventListener("ended", this.onPause);
        this.video.removeEventListener("waiting", this.onWaiting);
        this.video.removeEventListener("canplay", this.onCanPlay);
        this.video.removeEventListener("timeupdate", this.onTimeUpdate);
    }

    clear(): void {
        this.stop();

        for (const [elem, onError] of this.onError) {
            elem.removeEventListener("error", onError);
        }

        this.onError.clear();
        this.sourcesByURL.clear();
        this.sources.clear();
        this.potato.clear();

        this.video.src = "";
        this.audio.src = "";
        this.videoHadAudio = false;
        this._data = null;
        this._loaded = false;
    }

    async load(data: FullVideoRecord, prog?: IProgress): Promise<this> {
        this.clear();

        this._data = data;

        this.setTitle(data.title);

        this.fillSources(this.video, data.videos);
        this.fillSources(this.audio, data.audios);

        this.dispatchEvent(this.loadingEvt);

        await progressTasks(prog,
            (prog) => this.loadMediaElement(this.audio, prog),
            (prog) => this.loadMediaElement(this.video, prog));
        this._loaded = true;

        this.dispatchEvent(this.loadEvt);
        return this;
    }

    private fillSources(elem: HTMLMediaElement, formats: AudioRecord[]) {
        arraySortByKeyInPlace(formats, (f) => -f.resolution);
        for (const format of formats) {
            if (!Video_Vendor_Mpeg_Dash_Mpd.matches(format.contentType)) {
                this.sources.add(elem, format);
                this.sourcesByURL.set(format.url, format);
            }
        }
    }

    private createMediaElement<T extends HTMLMediaElement>(MediaElement: (...rest: ElementChild[]) => T, ...rest: ElementChild[]): T {
        return MediaElement(
            playsInline(true),
            autoPlay(false),
            loop(false),
            ...rest
        );
    }

    private async getMediaCapabilities<T extends AudioRecord>(source: T): Promise<MediaCapabilitiesDecodingInfo> {
        const config: MediaDecodingConfiguration = {
            type: "file"
        };

        if (isVideoRecord(source)) {
            config.video = {
                contentType: source.contentType,
                bitrate: source.vbr * 1024,
                framerate: source.fps,
                width: source.width,
                height: source.height
            }
        }
        else if (source.acodec !== "none") {
            config.audio = {
                contentType: source.contentType,
                bitrate: source.abr * 1024,
                samplerate: source.asr
            };
        }

        try {
            return await navigator.mediaCapabilities.decodingInfo(config)
        }
        catch {
            return {
                supported: true,
                powerEfficient: false,
                smooth: false,
                configuration: config
            };
        }
    }

    private hasSources(elem: HTMLMediaElement): boolean {
        return this.sources.get(elem).length > 0
            || this.potato.count(elem) > 0;
    }

    private consumeSource(elem: HTMLMediaElement): AudioRecord {
        const primary = this.sources.get(elem);
        const sources = primary.length > 0 ? primary : this.potato.get(elem);
        return sources.shift();
    }

    private async loadMediaElement(elem: HTMLMediaElement, prog?: IProgress): Promise<void> {
        if (isDefined(prog)) {
            prog.start();
        }

        if (this.onError.has(elem)) {
            elem.removeEventListener("error", this.onError.get(elem));
            this.onError.delete(elem);
        }

        while (this.hasSources(elem)) {
            const untested = this.sources.get(elem).length > 0;
            const source = this.consumeSource(elem);
            if (untested) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potato.add(elem, source);
                    continue;
                }
            }

            elem.src = source.url;
            const task = success<HTMLMediaElementEventMap>(elem, "canplaythrough", "error");
            elem.load();
            if (await task) {
                this.sources.get(elem).unshift(source);

                const onError = () => this.loadMediaElement(elem, prog);
                elem.addEventListener("error", onError);
                this.onError.set(elem, onError);
                this.videoHadAudio = this.videoHadAudio;

                if (isDefined(prog)) {
                    prog.end();
                }

                return;
            }
        }

        if (!this.hasSources(elem)) {
            const message = `No ${elem.tagName.toLowerCase()} sources`;
            if (isDefined(prog)) {
                prog.end(message);
            }
            throw new Error(message);
        }
    }

    get width() {
        return this.video.videoWidth;
    }

    get height() {
        return this.video.videoHeight;
    }

    get playbackState(): PlaybackState {
        if (isNullOrUndefined(this.data)) {
            return "empty";
        }

        if (!this.loaded) {
            return "loading";
        }

        if (this.video.error) {
            return "errored";
        }

        if (this.video.ended
            || this.video.paused
            && this.video.currentTime === 0) {
            return "stopped";
        }

        if (this.video.paused) {
            return "paused";
        }

        return "playing";
    }

    play(): Promise<void> {
        return this.video.play();
    }

    async playThrough(): Promise<void> {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.video.pause();
    }

    stop(): void {
        this.pause();
        this.video.currentTime = 0;
    }

    restart(): Promise<void> {
        this.stop();
        return this.play();
    }
}

