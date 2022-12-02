import { autoPlay, controls, loop, playsInline } from "@juniper-lib/dom/attrs";
import { Audio, ErsatzElement, mediaElementCanPlayThrough } from "@juniper-lib/dom/tags";
import { arrayClear, arrayReplace, arraySortByKeyInPlace } from "@juniper-lib/tslib/collections/arrays";
import { once } from "@juniper-lib/tslib/events/once";
import { AsyncCallback } from "@juniper-lib/tslib/identity";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";
import { URLBuilder } from "@juniper-lib/tslib/URLBuilder";
import { IDisposable } from "@juniper-lib/tslib/using";
import { AudioRecord, FullAudioRecord } from "../data";
import { MediaElementSource } from "../nodes";
import { audioReady, removeVertex } from "../util";
import { BaseAudioSource } from "./BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "./IPlayable";
import { IPlayer, MediaPlayerEvents, MediaPlayerLoadingEvent } from "./IPlayer";
import { PlaybackState } from "./PlaybackState";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";

export class AudioPlayer
    extends BaseAudioSource<MediaElementAudioSourceNode, MediaPlayerEvents>
    implements ErsatzElement<HTMLAudioElement>, IPlayer, IDisposable {

    private readonly cacheBustSources = new Map<FullAudioRecord | string, number>();

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

    private _data: FullAudioRecord | string = null;
    get data(): FullAudioRecord | string {
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
    private readonly potatoes = new Array<string>();

    constructor(audioCtx: AudioContext) {
        super("JuniperAudioPlayer", audioCtx, NoSpatializationNode.instance(audioCtx));

        this.element = Audio(
            playsInline(true),
            autoPlay(false),
            loop(false),
            controls(true)
        );

        this.input = MediaElementSource(
            "JuniperAudioPlayer-Input",
            audioCtx,
            { mediaElement: this.element });

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


        Object.assign(window, { audioPlayer: this });
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
        arrayClear(this.potatoes);

        this.element.src = "";
        this._data = null;
        this._loaded = false;
    }

    cacheBust(data: FullAudioRecord | string) {
        const curCount = this.cacheBustSources.get(data) || 0;
        this.cacheBustSources.set(data, curCount + 1);
    }

    async load(data: FullAudioRecord | string, prog?: IProgress): Promise<this> {
        this.clear();

        this._data = data;

        if (isString(data)) {
            this.setTitle(data);
            this.potatoes.push(data);
        }
        else {
            this.setTitle(data.title);
            arraySortByKeyInPlace(data.audios, (f) => -f.resolution);
            arrayReplace(this.sources, ...data.audios);
        }

        for (const audio of this.sources) {
            this.sourcesByURL.set(audio.url, audio);
        }

        if (!this.hasSources) {
            throw new Error("No audio sources");
        }

        this.dispatchEvent(this.loadingEvt);

        await this.loadAudio(prog);

        if (!this.hasSources) {
            throw new Error("No audio sources");
        }

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
            || this.potatoes.length > 0;
    }

    private async loadAudio(prog?: IProgress): Promise<void> {
        if (isDefined(prog)) {
            prog.start();
        }

        this.element.removeEventListener("error", this.onError);

        while (this.hasSources) {
            let url: string = null;
            const source = this.sources.shift();
            if (isDefined(source)) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potatoes.push(source.url);
                    continue;
                }
                else {
                    url = source.url;
                }
            }
            else {
                url = this.potatoes.shift();
            }

            const cacheV = this.cacheBustSources.get(this.data);
            if (isDefined(cacheV)) {
                const uri = new URLBuilder(url, location.href);
                uri.query("v", cacheV.toString());
                url = uri.toString();
            }
            this.element.src = url;
            this.element.load();
            if (await mediaElementCanPlayThrough(this.element)) {
                if (isDefined(source)) {
                    this.sources.unshift(source);
                }
                else {
                    this.potatoes.unshift(url);
                }

                this.element.addEventListener("error", this.onError);

                if (isDefined(prog)) {
                    prog.end();
                }

                return;
            }
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

    async play(): Promise<void> {
        await audioReady(this.audioCtx);
        await this.element.play();
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

