import { IDisposable, URLBuilder, arrayClear, arrayReplace, asyncCallback, isDefined, isNullOrUndefined, isString, once } from "@juniper-lib/util";
import { Audio, AutoPlay, Controls, Loop, mediaElementCanPlayThrough } from "@juniper-lib/dom";
import { IProgress } from "@juniper-lib/progress";
import { RELEASE_EVT } from "../AudioManager";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "../context/JuniperMediaElementAudioSourceNode";
import { AudioRecord, FullAudioRecord, audioRecordSorter } from "../data";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "./IPlayable";
import { IPlayer, MediaPlayerEvents, MediaPlayerLoadingEvent } from "./IPlayer";
import { PlaybackState } from "./PlaybackState";

export class AudioPlayer
    extends BaseAudioSource<MediaPlayerEvents<FullAudioRecord>>
    implements IPlayer<FullAudioRecord>, IDisposable {

    private readonly cacheBustSources = new Map<FullAudioRecord | string, number>();

    private readonly audioElement: HTMLAudioElement;

    private readonly loadingEvt: MediaPlayerLoadingEvent<FullAudioRecord, AudioPlayer>;
    private readonly loadEvt: MediaElementSourceLoadedEvent<IPlayer<FullAudioRecord>>;
    private readonly playEvt: MediaElementSourcePlayedEvent<IPlayer<FullAudioRecord>>;
    private readonly pauseEvt: MediaElementSourcePausedEvent<IPlayer<FullAudioRecord>>;
    private readonly stopEvt: MediaElementSourceStoppedEvent<IPlayer<FullAudioRecord>>;
    private readonly progEvt: MediaElementSourceProgressEvent<IPlayer<FullAudioRecord>>;

    private readonly onError: asyncCallback;
    private readonly onPlay: () => void;
    private readonly onCanPlay: () => void;
    private readonly onWaiting: () => void;
    private readonly onPause: (evt: Event) => void;
    private readonly onTimeUpdate: asyncCallback;

    private _data: FullAudioRecord | string = null;
    get data(): FullAudioRecord | string {
        return this._data;
    }

    private _loaded = false;
    public get loaded() {
        return this._loaded;
    }

    get title() {
        return this.audioElement.title;
    }

    protected setTitle(v: string): void {
        this.audioElement.title = v;
    }

    private readonly sourcesByURL = new Map<string, AudioRecord>();
    private readonly sources = new Array<AudioRecord>();
    private readonly potatoes = new Array<string>();

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer) {
        const mediaElement = Audio(
            AutoPlay(false),
            Loop(false),
            Controls(true)
        );

        const elementNode = new JuniperMediaElementAudioSourceNode(context, {
            mediaElement
        });

        elementNode.name = "JuniperAudioPlayer-Input";

        super("audio-player", context, spatializer, [], [elementNode]);

        elementNode.connect(this.volumeControl);

        this.audioElement = mediaElement;

        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.onPlay = async () => {
            this.enable();
            this.dispatchEvent(this.playEvt);
        };

        this.onPause = (evt: Event) => {
            this.disable();

            if (this.audioElement.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };

        this.onTimeUpdate = async () => {
            this.progEvt.value = this.audioElement.currentTime;
            this.progEvt.total = this.audioElement.duration;
            this.dispatchEvent(this.progEvt);
        };

        this.onError = () => this.loadAudio();

        this.audioElement.addEventListener("play", this.onPlay);
        this.audioElement.addEventListener("pause", this.onPause);
        this.audioElement.addEventListener("ended", this.onPause);
        this.audioElement.addEventListener("error", this.onError);
        this.audioElement.addEventListener("waiting", this.onWaiting);
        this.audioElement.addEventListener("canplay", this.onCanPlay);
        this.audioElement.addEventListener("timeupdate", this.onTimeUpdate);


        Object.assign(window, { audioPlayer: this });
    }

    get hasAudio() {
        const source = this.sourcesByURL.get(this.audioElement.src);
        return isDefined(source) && source.acodec !== "none"
            || isDefined(this.audioElement.audioTracks) && this.audioElement.audioTracks.length > 0
            || isDefined(this.audioElement.webkitAudioDecodedByteCount) && this.audioElement.webkitAudioDecodedByteCount > 0
            || isDefined(this.audioElement.mozHasAudio) && this.audioElement.mozHasAudio;
    }

    protected override onDisposing(): void {
        super.onDisposing();
        this.clear();

        this.audioElement.removeEventListener("play", this.onPlay);
        this.audioElement.removeEventListener("pause", this.onPause);
        this.audioElement.removeEventListener("ended", this.onPause);
        this.audioElement.removeEventListener("error", this.onError);
        this.audioElement.removeEventListener("waiting", this.onWaiting);
        this.audioElement.removeEventListener("canplay", this.onCanPlay);
        this.audioElement.removeEventListener("timeupdate", this.onTimeUpdate);
        this.audioElement.dispatchEvent(RELEASE_EVT);
    }

    clear(): void {
        this.stop();

        this.sourcesByURL.clear();
        arrayClear(this.sources);
        arrayClear(this.potatoes);

        this.audioElement.src = "";
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
            data.audios.sort(audioRecordSorter);
            arrayReplace(this.sources, data.audios);
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
        };

        try {
            return await navigator.mediaCapabilities.decodingInfo(config);
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

        this.audioElement.removeEventListener("error", this.onError);

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
            this.audioElement.src = url;
            this.audioElement.load();
            if (await mediaElementCanPlayThrough(this.audioElement)) {
                if (isDefined(source)) {
                    this.sources.unshift(source);
                }
                else {
                    this.potatoes.unshift(url);
                }

                this.audioElement.addEventListener("error", this.onError);

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

        if (this.audioElement.error) {
            return "errored";
        }

        if (this.audioElement.ended
            || this.audioElement.paused
            && this.audioElement.currentTime === 0) {
            return "stopped";
        }

        if (this.audioElement.paused) {
            return "paused";
        }

        return "playing";
    }

    async play(): Promise<void> {
        await this.context.ready;
        await this.audioElement.play();
    }

    async playThrough(): Promise<void> {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.audioElement.pause();
    }

    stop(): void {
        this.pause();
        this.audioElement.currentTime = 0;
    }

    restart(): Promise<void> {
        this.stop();
        return this.play();
    }

    get element() {
        return this.audioElement;
    }
}

