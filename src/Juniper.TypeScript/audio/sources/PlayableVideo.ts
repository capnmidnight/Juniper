import { autoPlay, controls, loop, playsInline, src, title, type } from "juniper-dom/attrs";
import { cursor, display, styles } from "juniper-dom/css";
import { Audio, Div, elementApply, ElementChild, elementSetDisplay, ErsatzElement, Img, mediaElementCanPlayThrough, Source, Video } from "juniper-dom/tags";
import { arrayRemove, arrayScan, arraySortByKeyInPlace, BaseProgress, identity, IProgress, isDefined, once, progressSplit, TypedEventBase } from "juniper-tslib";
import { MediaRecord } from "../YouTubeProxy";
import { IPlayable, MediaElementSourceEvents, MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "./IPlayable";

async function checkMediaType(f: MediaRecord): Promise<MediaRecord> {
    const config: MediaDecodingConfiguration = {
        type: "file"
    };

    try {
        if (f.data.vcodec !== "none") {
            config.video = {
                contentType: f.content_type,
                bitrate: f.data.vbr * 1024,
                framerate: f.data.fps,
                width: f.width,
                height: f.height
            }
        }
        else if (f.data.acodec !== "none") {
            config.audio = {
                contentType: f.content_type,
                bitrate: f.data.abr * 1024,
                samplerate: f.data.asr
            };
        }

        const info = await navigator.mediaCapabilities.decodingInfo(config);

        if (info.smooth) {
            return f;
        }
    }
    catch (exp) {
        console.warn({ config, f, exp });
    }

    return null;
}

function byResolution(f: MediaRecord) { return -f.resolution; }

export class PlayableVideo
    extends TypedEventBase<MediaElementSourceEvents>
    implements ErsatzElement, IPlayable {

    private readonly loadEvt: MediaElementSourceLoadedEvent;
    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;
    private readonly progress: IProgress;
    private readonly loadingTask: Promise<this>;

    private loaded = false;

    readonly element: HTMLElement;

    private readonly onPlay: () => void;
    private readonly onSeeked: () => void;
    private readonly onCanPlay: () => void;
    private readonly onWaiting: () => void;
    private readonly onEnded: (evt: Event) => void;
    private readonly onPause: (evt: Event) => void;
    private readonly onAudioError: () => Promise<void>;
    private readonly onVideoError: () => Promise<void>;
    private readonly onTimeUpdate: () => Promise<void>;

    private _video: HTMLVideoElement;
    get video() { return this._video; }

    private _audio: HTMLAudioElement;
    get audio() { return this._audio; }

    readonly thumbnail: HTMLImageElement;

    private videoSources: HTMLSourceElement[] = null;
    private audioSources: HTMLSourceElement[] = null;

    constructor(t: string,
        private readonly videoFormats: MediaRecord[],
        private readonly audioFormats: MediaRecord[],
        private readonly thumbnailSrc: string) {
        super();

        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        arraySortByKeyInPlace(videoFormats, byResolution);
        arraySortByKeyInPlace(audioFormats, byResolution);

        this.element = Div(
            styles(display("inline-block")),
            this.thumbnail = this.createImage(t),
            this._video = this.createVideo(t),
            this._audio = this.createAudio(t)
        );


        const showVideo = (v: boolean) => {
            elementSetDisplay(this.video, v, "inline-block");
            elementSetDisplay(this.thumbnail, !v, "inline-block");
        };

        this.onSeeked = () => {
            this.audio.currentTime = this.video.currentTime;
        };

        this.onPlay = () => {
            showVideo(true);
            this.onSeeked();
            this.audio.play();
            this.dispatchEvent(this.playEvt)
        };

        this.onPause = (evt: Event) => {
            this.onSeeked();
            this.audio.pause();
            if (this.video.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };

        this.onEnded = (evt: Event) => {
            showVideo(false);
            this.onPause(evt);
        };

        let wasWaiting = false;
        this.onWaiting = () => {
            wasWaiting = true;
            this.audio.pause();
        };

        this.onCanPlay = () => {
            if (wasWaiting) {
                wasWaiting = false;
                this.audio.play();
            }
        };

        let errorCorrecting = false;
        this.onTimeUpdate = async () => {
            if (!errorCorrecting) {
                const quality = this.video.getVideoPlaybackQuality();
                if (quality.totalVideoFrames === 0) {
                    errorCorrecting = true;
                    await this.removeCurrentSource(this.video, this.videoFormats, this.videoSources);
                    errorCorrecting = false;
                }
                else {
                    const delta = this.video.currentTime - this.audio.currentTime;
                    if (Math.abs(delta) > 0.25) {
                        this.audio.currentTime = this.video.currentTime;
                    }
                }
                this.progEvt.value = this.video.currentTime;
                this.progEvt.total = this.video.duration;
                this.dispatchEvent(this.progEvt);
            }
        };

        this.onVideoError = () => this.removeCurrentSource(this.video, this.videoFormats, this.videoSources);
        this.onAudioError = () => this.removeCurrentSource(this.audio, this.audioFormats, this.audioSources);

        this.thumbnail.addEventListener("click", () => this.play());
        this.bindVideoEvents();
        this.bindAudioEvents();

        showVideo(false);

        this.progress = new BaseProgress();
        const progs = progressSplit(this.progress, 3);
        this.loadingTask = Promise.all([
            this.loadThumbnail(progs.shift()),
            this.loadVideo(progs.shift()),
            this.loadAudio(progs.shift())
        ]).then(() => {
            this.loaded = true;
            this.dispatchEvent(this.loadEvt);
            return this;
        });
    }

    private async createSources(formats: MediaRecord[]): Promise<HTMLSourceElement[]> {
        return (await Promise.all(formats
            .map(checkMediaType)))
            .filter(identity)
            .map(f => Source(
                type(f.content_type),
                src(f.url)
            ));
    }

    load(prog?: IProgress): Promise<this> {
        if (isDefined(prog)) {
            this.progress.attach(prog);
        }

        return this.loadingTask;
    }

    private async removeCurrentSource(element: HTMLMediaElement, formats: MediaRecord[], sources: HTMLSourceElement[]): Promise<void> {
        const format = arrayScan(formats, f => f.url === element.currentSrc);
        const source = arrayScan(sources, s => s.src === element.currentSrc);
        if (format && source) {
            const currentTime = element.currentTime;
            this.pause();

            const isVideo = element === this.video;
            const newElement = isVideo
                ? this.createVideo(element.title)
                : this.createAudio(element.title);

            arrayRemove(formats, format);
            arrayRemove(sources, source);
            source.remove();

            await (isVideo ? this.loadVideo() : this.loadAudio());

            if (isVideo) {
                this.unbindVideoEvents();
                this._video = newElement as HTMLVideoElement;
                this.bindVideoEvents();
            }
            else {
                this.unbindAudioEvents();
                this._audio = newElement as HTMLAudioElement;
                this.bindAudioEvents();
            }

            element.replaceWith(newElement);
            newElement.currentTime = currentTime;

            await this.play();
        }
    }

    private createElement<T extends HTMLElement>(Element: (...rest: ElementChild[]) => T, t: string, ...rest: ElementChild[]): T {
        return Element(
            title(t),
            ...rest);
    }

    private createMediaElement<T extends HTMLMediaElement>(MediaElement: (...rest: ElementChild[]) => T, t: string, ...rest: ElementChild[]): T {
        return this.createElement(
            MediaElement,
            t,
            playsInline(true),
            autoPlay(false),
            loop(false),
            ...rest
        );
    }

    private createImage(t: string): HTMLImageElement {
        return this.createElement(Img, t, styles(cursor("pointer")));
    }

    private createAudio(t: string): HTMLAudioElement {
        return this.createMediaElement(Audio, t, controls(false));
    }

    private createVideo(t: string): HTMLVideoElement {
        return this.createMediaElement(Video, t, controls(true));
    }

    private bindAudioEvents() {
        this.audio.addEventListener("error", this.onAudioError);
    }

    private unbindAudioEvents() {
        this.audio.removeEventListener("error", this.onAudioError);
    }

    private bindVideoEvents() {
        this.video.addEventListener("seeked", this.onSeeked);
        this.video.addEventListener("play", this.onPlay);
        this.video.addEventListener("pause", this.onPause);
        this.video.addEventListener("ended", this.onEnded);
        this.video.addEventListener("waiting", this.onWaiting);
        this.video.addEventListener("canplay", this.onCanPlay);
        this.video.addEventListener("timeupdate", this.onTimeUpdate);
        this.video.addEventListener("error", this.onVideoError);
    }

    private unbindVideoEvents() {
        this.video.removeEventListener("seeked", this.onSeeked);
        this.video.removeEventListener("play", this.onPlay);
        this.video.removeEventListener("pause", this.onPause);
        this.video.removeEventListener("ended", this.onEnded);
        this.video.removeEventListener("waiting", this.onWaiting);
        this.video.removeEventListener("canplay", this.onCanPlay);
        this.video.removeEventListener("timeupdate", this.onTimeUpdate);
        this.video.removeEventListener("error", this.onVideoError);
    }

    private async loadThumbnail(prog?: IProgress): Promise<void> {
        if (isDefined(prog)) {
            prog.report(0, 1);
        }
        const task = once<GlobalEventHandlersEventMap, "load">(this.thumbnail, "load");
        this.thumbnail.src = this.thumbnailSrc;
        await task;
        if (isDefined(prog)) {
            prog.end();
        }
    }

    private async loadMediaElement(elem: HTMLMediaElement, sources: HTMLSourceElement[], prog?: IProgress): Promise<void> {
        const task = mediaElementCanPlayThrough(elem, prog);
        elementApply(elem, ...sources);
        await task;
    }

    private async loadAudio(prog?: IProgress): Promise<void> {
        this.videoSources = await this.createSources(this.videoFormats);
        return await this.loadMediaElement(this.audio, this.audioSources, prog);
    }

    private async loadVideo(prog?: IProgress): Promise<void> {
        this.videoSources = await this.createSources(this.videoFormats);
        return await this.loadMediaElement(this.video, this.videoSources, prog);
    }

    get audioSource(): HTMLMediaElement {
        return this.audioFormats.length > 0
            ? this.audio
            : this.video;
    }

    get width() {
        return this.video.videoWidth;
    }

    get height() {
        return this.video.videoHeight;
    }

    get playbackState(): PlaybackState {
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

    restart(): void {
        this.stop();
        this.play();
    }
}
