import { autoPlay, controls, loop, playsInline, src, title, type } from "juniper-dom/attrs";
import { cursor, display, styles } from "juniper-dom/css";
import { Audio, Div, elementApply, ElementChild, elementSetDisplay, ErsatzElement, Img, mediaElementCanPlayThrough, Source, Video } from "juniper-dom/tags";
import { mediaTypeParse } from "juniper-mediatypes";
import { arraySortByKeyInPlace, BaseProgress, identity, IProgress, isDefined, once, PriorityList, progressSplit, TypedEventBase } from "juniper-tslib";
import { IPlayable, MediaElementSourceEvents, MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "./IPlayable";

async function checkMediaType(f: YTMediaEntry): Promise<YTMediaEntry> {
    const config: MediaDecodingConfiguration = {
        type: "file"
    };

    try {
        if (f.data.vcodec !== "none") {
            config.video = {
                contentType: f.content_type,
                framerate: f.data.fps,
                bitrate: f.data.vbr * 1024,
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

    private _video: HTMLVideoElement;
    get video() { return this._video; }

    private _audio: HTMLAudioElement;
    get audio() { return this._audio; }

    readonly thumbnail: HTMLImageElement;
    readonly formatsByType = new PriorityList<string, YTMediaEntry>();
    readonly formatsBySrc = new Map<string, YTMediaEntry>();
    readonly sourcesBySrc = new Map<string, HTMLSourceElement>();

    constructor(t: string,
        formats: YTMediaEntry[],
        private readonly thumbnailSrc: string) {
        super();

        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        for (const f of formats) {
            const t = mediaTypeParse(f.content_type).typeName;
            const source = Source(
                type(f.content_type),
                src(f.url)
            );
            this.formatsByType.add(t, f);
            this.formatsBySrc.set(f.url, f);
            this.sourcesBySrc.set(f.url, source);
        }

        const byResolution = (f: YTMediaEntry) => -f.resolution;

        arraySortByKeyInPlace(
            this.formatsByType.get("video"),
            byResolution);
        arraySortByKeyInPlace(
            this.formatsByType.get("audio"),
            byResolution);

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

        showVideo(false);

        const onSeeked = () => {
            this.audio.currentTime = this.video.currentTime;
        };
        this.video.addEventListener("seeked", onSeeked);

        this.video.addEventListener("play", () => {
            showVideo(true);
            onSeeked();
            this.audio.play();
            this.dispatchEvent(this.playEvt)
        });

        const onPause = (evt: Event) => {
            onSeeked();
            this.audio.pause();
            if (this.video.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };
        this.video.addEventListener("pause", onPause);

        this.video.addEventListener("ended", (evt: Event) => {
            showVideo(false);
            onPause(evt);
        });

        let wasWaiting = false;
        this.video.addEventListener("waiting", () => {
            wasWaiting = true;
            this.audio.pause();
        });

        this.video.addEventListener("canplay", () => {
            if (wasWaiting) {
                wasWaiting = false;
                this.audio.play();
            }
        });

        

        this.video.addEventListener("timeupdate", async () => {
            const quality = this.video.getVideoPlaybackQuality();
            if (quality.totalVideoFrames === 0) {
                await this.removeCurrentSource(this.video);
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
        });

        this.video.addEventListener("error", () => this.removeCurrentSource(this.video));
        this.audio.addEventListener("error", () => this.removeCurrentSource(this.audio));
        this.thumbnail.addEventListener("click", () => this.play());

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

    load(prog?: IProgress): Promise<this> {
        if (isDefined(prog)) {
            this.progress.attach(prog);
        }

        return this.loadingTask;
    }

    private async removeCurrentSource(element: HTMLMediaElement): Promise<void> {
        const format = this.formatsBySrc.get(element.currentSrc);
        const source = this.sourcesBySrc.get(element.currentSrc);
        if (format && source) {
            const currentTime = element.currentTime;
            this.pause();
            const type = element.tagName.toLowerCase();
            this.formatsByType.remove(type, format);
            this.formatsBySrc.delete(element.currentSrc);
            this.sourcesBySrc.delete(element.currentSrc);

            const isVideo = element === this.video
            const newElement = isVideo
                ? this.createVideo(element.title)
                : this.createAudio(element.title);

            element.replaceWith(newElement);

            await (isVideo ? this.loadVideo() : this.loadAudio());

            element.currentTime = currentTime;
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

    private async loadMediaElement(type: string, elem: HTMLMediaElement, prog?: IProgress): Promise<void> {
        const task = mediaElementCanPlayThrough(elem, prog);
        const formats = (await Promise.all(
            this.formatsByType
                .get(type)
                .map(checkMediaType)))
            .filter(identity);
        const sources = formats.map(f =>
            this.sourcesBySrc.get(f.url));
        elementApply(elem, ...sources);
        await task;
    }

    private loadAudio(prog?: IProgress): Promise<void> {
        return this.loadMediaElement("audio", this.audio, prog);
    }

    private loadVideo(prog?: IProgress): Promise<void> {
        return this.loadMediaElement("video", this.video, prog);
    }

    get audioSource(): HTMLMediaElement {
        return this.formatsByType.get("audio").length > 0
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
