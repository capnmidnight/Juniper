import { RELEASE_EVT } from "@juniper-lib/audio/dist/AudioManager";
import { JuniperAudioContext } from "@juniper-lib/audio/dist/context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "@juniper-lib/audio/dist/context/JuniperMediaElementAudioSourceNode";
import { AudioRecord, audioRecordSorter } from "@juniper-lib/audio/dist/data";
import { BaseAudioSource } from "@juniper-lib/audio/dist/sources/BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "@juniper-lib/audio/dist/sources/IPlayable";
import { IPlayer, MediaPlayerEvents, MediaPlayerLoadingEvent } from "@juniper-lib/audio/dist/sources/IPlayer";
import { PlaybackState } from "@juniper-lib/audio/dist/sources/PlaybackState";
import { BaseSpatializer } from "@juniper-lib/audio/dist/spatializers/BaseSpatializer";
import { PriorityList } from "@juniper-lib/collections/dist/PriorityList";
import { AutoPlay, Controls, Loop } from "@juniper-lib/dom/dist/attrs";
import { Audio, ElementChild, Video, mediaElementCanPlayThrough } from "@juniper-lib/dom/dist/tags";
import { once } from "@juniper-lib/events/dist/once";
import { Video_Vendor_Mpeg_Dash_Mpd } from "@juniper-lib/mediatypes/dist";
import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { progressTasks } from "@juniper-lib/progress/dist/progressTasks";
import { AsyncCallback } from "@juniper-lib/tslib/dist/identity";
import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/dist/using";
import { FullVideoRecord, isVideoRecord } from "./data";
export abstract class BaseVideoPlayer
    extends BaseAudioSource<MediaPlayerEvents>
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

    private wasUsingAudioElement = false;
    private nextStartTime: number = null;

    private _data: FullVideoRecord | string = null;
    get data(): FullVideoRecord | string {
        return this._data;
    }

    private _loaded = false;
    public get loaded() {
        return this._loaded;
    }

    protected readonly video: HTMLVideoElement;
    protected readonly audio: HTMLAudioElement;

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
    private readonly potatoes = new PriorityList<HTMLMediaElement, string>();

    constructor(
        type: string,
        context: JuniperAudioContext,
        spatializer: BaseSpatializer) {

        const video = BaseVideoPlayer.createMediaElement(Video, Controls(true));
        const audio = BaseVideoPlayer.createMediaElement(Audio, Controls(false));

        const videoNode = new JuniperMediaElementAudioSourceNode(
            context, {
                mediaElement: video
            });
        videoNode.name = `${type}-video`;

        const audioNode = new JuniperMediaElementAudioSourceNode(
            context, {
                mediaElement: audio
            });
        audioNode.name = `${type}-audio`;

        super(type, context, spatializer, [], [videoNode, audioNode]);

        videoNode.connect(this.volumeControl);
        audioNode.connect(this.volumeControl);

        this.video = video;
        this.audio = audio;

        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.onSeeked = () => {
            if (this.useAudioElement) {
                this.audio.currentTime = this.video.currentTime;
            }
        };

        this.onPlay = async () => {
            this.onSeeked();
            if (this.useAudioElement) {
                await this.context.ready;
                await this.audio.play();
            }
            this.dispatchEvent(this.playEvt);
        };

        this.onPause = (evt: Event) => {
            if (this.useAudioElement) {
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
            if (this.useAudioElement) {
                wasWaiting = true;
                this.audio.pause();
            }
        };

        this.onCanPlay = async () => {
            if (this.useAudioElement && wasWaiting) {
                await this.context.ready;
                await this.audio.play();
                wasWaiting = false;
            }
        };

        this.wasUsingAudioElement = false;
        this.onTimeUpdate = async () => {
            const quality = this.video.getVideoPlaybackQuality();
            if (quality.totalVideoFrames === 0) {
                const onError = this.onError.get(this.video);
                if (isDefined(onError)) {
                    await onError();
                }
            }
            else if (this.useAudioElement) {
                this.wasUsingAudioElement = false;
                const delta = this.video.currentTime - this.audio.currentTime;
                if (Math.abs(delta) > 0.25) {
                    this.audio.currentTime = this.video.currentTime;
                }
            }
            else if (!this.wasUsingAudioElement) {
                this.wasUsingAudioElement = true;
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

    private get useAudioElement(): boolean {
        return !this.elementHasAudio(this.video)
            && this.elementHasAudio(this.audio);
    }

    protected override onDisposing(): void {
        this.clear();
        this.video.removeEventListener("seeked", this.onSeeked);
        this.video.removeEventListener("play", this.onPlay);
        this.video.removeEventListener("pause", this.onPause);
        this.video.removeEventListener("ended", this.onPause);
        this.video.removeEventListener("waiting", this.onWaiting);
        this.video.removeEventListener("canplay", this.onCanPlay);
        this.video.removeEventListener("timeupdate", this.onTimeUpdate);
        super.onDisposing();
        this.audio.dispatchEvent(RELEASE_EVT);
        this.video.dispatchEvent(RELEASE_EVT);
    }

    clear(): void {
        this.stop();

        for (const [elem, onError] of this.onError) {
            elem.removeEventListener("error", onError);
        }

        this.onError.clear();
        this.sourcesByURL.clear();
        this.sources.clear();
        this.potatoes.clear();

        this.video.src = "";
        this.audio.src = "";
        this.wasUsingAudioElement = false;
        this._data = null;
        this._loaded = false;
    }

    async load(data: FullVideoRecord | string, prog?: IProgress): Promise<this> {
        this.clear();

        this._data = data;

        if (isString(data)) {
            this.setTitle(data);
            this.potatoes.add(this.video, data);
        }
        else {
            this.setTitle(data.title);
            this.fillSources(this.video, data.videos);
            this.fillSources(this.audio, data.audios);
        }

        if (!this.hasSources(this.video)) {
            throw new Error("No video sources found");
        }

        this.dispatchEvent(this.loadingEvt);

        await progressTasks(prog,
            (prog) => this.loadMediaElement(this.audio, prog),
            (prog) => this.loadMediaElement(this.video, prog));

        if (isString(data)) {
            this.nextStartTime = null;
        }
        else {
            this.nextStartTime = data.startTime;
        }

        if (!this.hasSources(this.video)) {
            throw new Error("No video playable sources");
        }

        this._loaded = true;

        this.dispatchEvent(this.loadEvt);
        return this;
    }

    private fillSources(elem: HTMLMediaElement, formats: AudioRecord[]) {
        formats.sort(audioRecordSorter);
        for (const format of formats) {
            if (!Video_Vendor_Mpeg_Dash_Mpd.matches(format.contentType)) {
                this.sources.add(elem, format);
                this.sourcesByURL.set(format.url, format);
            }
        }
    }

    private static createMediaElement<T extends HTMLMediaElement>(MediaElement: (...rest: ElementChild[]) => T, ...rest: ElementChild[]): T {
        return MediaElement(
            AutoPlay(false),
            Loop(false),
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
            };
        }
        else if (source.acodec !== "none") {
            config.audio = {
                contentType: source.contentType,
                bitrate: source.abr * 1024,
                samplerate: source.asr
            };
        }

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

    private hasSources(elem: HTMLMediaElement): boolean {
        return this.sources.get(elem).length > 0
            || this.potatoes.count(elem) > 0;
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
            let url: string = null;
            const source = this.sources.get(elem).shift();
            if (isDefined(source)) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potatoes.add(elem, source.url);
                    continue;
                }
                else {
                    url = source.url;
                }
            }
            else {
                url = this.potatoes.get(elem).shift();
            }

            elem.src = url;
            elem.load();
            if (await mediaElementCanPlayThrough(elem)) {
                if (isDefined(source)) {
                    this.sources.get(elem).unshift(source);
                }
                else {
                    this.potatoes.get(elem).unshift(url);
                }

                const onError = () => this.loadMediaElement(elem, prog);
                elem.addEventListener("error", onError);
                this.onError.set(elem, onError);
                this.wasUsingAudioElement = this.wasUsingAudioElement;

                if (isDefined(prog)) {
                    prog.end();
                }

                return;
            }
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

    async play(): Promise<void> {
        await this.context.ready;
        if (isDefined(this.nextStartTime) && this.nextStartTime > 0) {
            this.video.pause();
            this.video.currentTime = this.nextStartTime;
            this.nextStartTime = null;
        }
        await this.video.play();
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

