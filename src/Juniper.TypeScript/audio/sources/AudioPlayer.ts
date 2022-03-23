import { autoPlay, controls, loop, playsInline } from "juniper-dom/attrs";
import { Audio, ErsatzElement } from "juniper-dom/tags";
import { arrayClear, arrayReplace, arraySortByKeyInPlace, AsyncCallback, IDisposable, IProgress, isDefined, isNullOrUndefined, once, success } from "juniper-tslib";
import { AudioRecord, FullAudioRecord } from "../data";
import { MediaElementSource, removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "./IPlayable";
import { IPlayer, MediaPlayerEvents, MediaPlayerLoadingEvent } from "./IPlayer";
import { PlaybackState } from "./PlaybackState";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";

export class AudioPlayer
    extends BaseAudioSource<MediaElementAudioSourceNode, MediaPlayerEvents>
    implements ErsatzElement, IPlayer, IDisposable {

    private readonly loadingEvt: MediaPlayerLoadingEvent;
    private readonly loadEvt: MediaElementSourceLoadedEvent<IPlayer>;
    private readonly playEvt: MediaElementSourcePlayedEvent<IPlayer>;
    private readonly pauseEvt: MediaElementSourcePausedEvent<IPlayer>;
    private readonly stopEvt: MediaElementSourceStoppedEvent<IPlayer>;
    private readonly progEvt: MediaElementSourceProgressEvent<IPlayer>;

    private readonly onError: AsyncCallback;
    private readonly onPlay: () => void;
    private readonly onCanPlay: () => void;
    private readonly onWaiting: () => void;
    private readonly onPause: (evt: Event) => void;
    private readonly onTimeUpdate: AsyncCallback;

    private _data: FullAudioRecord = null;
    get data(): FullAudioRecord {
        return this._data;
    }

    private _loaded = false;
    public get loaded() {
        return this._loaded;
    }

    readonly element: HTMLAudioElement;
    private readonly audioSource: MediaElementAudioSourceNode;

    get title() {
        return this.element.title;
    }

    protected setTitle(v: string): void {
        this.element.title = v;
    }

    private readonly sourcesByURL = new Map<string, AudioRecord>();
    private readonly sources = new Array<AudioRecord>();
    private readonly potato = new Array<AudioRecord>();

    constructor(audioCtx: AudioContext) {
        super("JuniperAudioPlayer", audioCtx, NoSpatializationNode.instance(audioCtx));

        this.element = Audio(
            playsInline(true),
            autoPlay(false),
            loop(false),
            controls(false)
        );

        this.input = MediaElementSource(
            "JuniperAudioPlayer-Input",
            audioCtx,
            this.element);

        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.onPlay = async () => {
            this.connect();
            this.dispatchEvent(this.playEvt)
        };

        this.onPause = (evt: Event) => {
            this.disconnect();

            if (this.element.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };

        this.onTimeUpdate = async () => {
            this.progEvt.value = this.element.currentTime;
            this.progEvt.total = this.element.duration;
            this.dispatchEvent(this.progEvt);
        };

        this.onError = () => this.loadAudio();

        this.element.addEventListener("play", this.onPlay);
        this.element.addEventListener("pause", this.onPause);
        this.element.addEventListener("ended", this.onPause);
        this.element.addEventListener("error", this.onError);
        this.element.addEventListener("waiting", this.onWaiting);
        this.element.addEventListener("canplay", this.onCanPlay);
        this.element.addEventListener("timeupdate", this.onTimeUpdate);


        Object.assign(window, { player: this });
    }

    get hasAudio() {
        const source = this.sourcesByURL.get(this.element.src);
        return isDefined(source) && source.acodec !== "none"
            || isDefined(this.element.audioTracks) && this.element.audioTracks.length > 0
            || isDefined(this.element.webkitAudioDecodedByteCount) && this.element.webkitAudioDecodedByteCount > 0
            || isDefined(this.element.mozHasAudio) && this.element.mozHasAudio;
    }

    protected override onDisposing(): void {
        super.onDisposing();
        this.clear();
        removeVertex(this.input);
        removeVertex(this.audioSource);

        this.element.removeEventListener("play", this.onPlay);
        this.element.removeEventListener("pause", this.onPause);
        this.element.removeEventListener("ended", this.onPause);
        this.element.removeEventListener("error", this.onError);
        this.element.removeEventListener("waiting", this.onWaiting);
        this.element.removeEventListener("canplay", this.onCanPlay);
        this.element.removeEventListener("timeupdate", this.onTimeUpdate);
    }

    clear(): void {
        this.stop();

        this.sourcesByURL.clear();
        arrayClear(this.sources);
        arrayClear(this.potato);

        this.element.src = "";
        this._data = null;
        this._loaded = false;
    }

    async load(data: FullAudioRecord, prog?: IProgress): Promise<this> {
        this.clear();

        this._data = data;

        this.setTitle(data.title);

        arraySortByKeyInPlace(data.audios, (f) => -f.resolution);

        arrayReplace(this.sources, ...data.audios);
        for (const audio of data.audios) {
            this.sourcesByURL.set(audio.url, audio);
        }

        this.dispatchEvent(this.loadingEvt);
        await this.loadAudio(prog);
        this._loaded = true;
        this.dispatchEvent(this.loadEvt);
        return this;
    }

    private async getMediaCapabilities<T extends AudioRecord>(source: T): Promise<MediaCapabilitiesDecodingInfo> {
        const config: MediaDecodingConfiguration = {
            type: "file",
            audio: {
                contentType: source.contentType,
                bitrate: source.abr * 1024,
                samplerate: source.asr
            }
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

    private get hasSources(): boolean {
        return this.sources.length > 0
            || this.potato.length > 0;
    }

    private consumeSource(): AudioRecord {
        const sources = this.sources.length > 0
            ? this.sources
            : this.potato;
        return sources.shift();
    }

    private async loadAudio(prog?: IProgress): Promise<void> {
        if (isDefined(prog)) {
            prog.start();
        }

        this.element.removeEventListener("error", this.onError);

        while (this.hasSources) {
            const untested = this.sources.length > 0;
            const source = this.consumeSource();
            if (untested) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potato.push(source);
                    continue;
                }
            }

            this.element.src = source.url;
            const task = success<HTMLMediaElementEventMap>(this.element, "canplaythrough", "error");
            this.element.load();
            if (await task) {
                this.sources.unshift(source);

                this.element.addEventListener("error", this.onError);

                if (isDefined(prog)) {
                    prog.end();
                }

                return;
            }
        }

        if (!this.hasSources) {
            const message = `No ${this.element.tagName.toLowerCase()} sources`;
            if (isDefined(prog)) {
                prog.end(message);
            }
            throw new Error(message);
        }
    }

    get playbackState(): PlaybackState {
        if (isNullOrUndefined(this.data)) {
            return "empty";
        }

        if (!this.loaded) {
            return "loading";
        }

        if (this.element.error) {
            return "errored";
        }

        if (this.element.ended
            || this.element.paused
        && this.element.currentTime === 0) {
            return "stopped";
        }

        if (this.element.paused) {
            return "paused";
        }

        return "playing";
    }

    play(): Promise<void> {
        return this.element.play();
    }

    async playThrough(): Promise<void> {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.element.pause();
    }

    stop(): void {
        this.pause();
        this.element.currentTime = 0;
    }

    restart(): Promise<void> {
        this.stop();
        return this.play();
    }
}

