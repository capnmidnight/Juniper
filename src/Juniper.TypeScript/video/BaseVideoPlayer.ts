import { IPlayable, MediaElementSourceEvents, MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "juniper-audio/sources/IPlayable";
import { autoPlay, controls, loop, playsInline } from "juniper-dom/attrs";
import { Audio, ElementChild, mediaElementCanPlayThrough, Video } from "juniper-dom/tags";
import { arrayReplace, arraySortByKeyInPlace, identity, IProgress, once, progressOfArray, progressSplit, TypedEventBase } from "juniper-tslib";
import { AudioRecord, FullVideoRecord, isVideoRecord, VideoRecord } from "./data";

export abstract class BaseVideoPlayer
    extends TypedEventBase<MediaElementSourceEvents>
    implements IPlayable {

    private readonly loadEvt: MediaElementSourceLoadedEvent;
    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;

    private readonly onPlay: () => void;
    private readonly onSeeked: () => void;
    private readonly onCanPlay: () => void;
    private readonly onWaiting: () => void;
    private readonly onPause: (evt: Event) => void;
    private readonly onAudioError: () => Promise<void>;
    private readonly onVideoError: () => Promise<void>;
    private readonly onTimeUpdate: () => Promise<void>;

    private loaded = false;

    readonly video: HTMLVideoElement;
    readonly audio: HTMLAudioElement;

    get title() {
        return this.video.title;
    }

    protected setTitle(v: string): void {
        this.video.title = v;
        this.audio.title = v;
    }

    private readonly videoFormats = new Array<VideoRecord>();
    private readonly audioFormats = new Array<AudioRecord>();

    constructor() {
        super();

        this.video = this.createMediaElement(Video, controls(true));
        this.audio = this.createMediaElement(Audio, controls(false));

        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.onSeeked = () => {
            this.audio.currentTime = this.video.currentTime;
        };

        this.onPlay = () => {
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

        this.onVideoError = async () => {
            errorCorrecting = true;
            this.videoFormats.shift();
            await this.loadVideo();
            errorCorrecting = false;
        };

        this.onAudioError = async () => {
            errorCorrecting = true;
            this.audioFormats.shift();
            await this.loadAudio();
            errorCorrecting = false;
        };

        this.onTimeUpdate = async () => {
            if (!errorCorrecting) {
                const quality = this.video.getVideoPlaybackQuality();
                if (quality.totalVideoFrames === 0) {
                    await this.onVideoError();
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

        this.audio.addEventListener("error", this.onAudioError);
        this.video.addEventListener("seeked", this.onSeeked);
        this.video.addEventListener("play", this.onPlay);
        this.video.addEventListener("pause", this.onPause);
        this.video.addEventListener("ended", this.onPause);
        this.video.addEventListener("waiting", this.onWaiting);
        this.video.addEventListener("canplay", this.onCanPlay);
        this.video.addEventListener("timeupdate", this.onTimeUpdate);
        this.video.addEventListener("error", this.onVideoError);
    }

    private async checkSources<T extends AudioRecord>(formats: T[], prog: IProgress): Promise<void> {
        const good = (await progressOfArray(prog, formats, async (f) => {
            const config: MediaDecodingConfiguration = {
                type: "file"
            };

            try {
                if (isVideoRecord(f)) {
                    config.video = {
                        contentType: f.contentType,
                        bitrate: f.vbr * 1024,
                        framerate: f.fps,
                        width: f.width,
                        height: f.height
                    }
                }
                else if (f.acodec !== "none") {
                    config.audio = {
                        contentType: f.contentType,
                        bitrate: f.abr * 1024,
                        samplerate: f.asr
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
        }))
            .filter(identity);
        arrayReplace(formats, ...good);
        arraySortByKeyInPlace(formats, (f) => -f.resolution);
    }

    async load(data: FullVideoRecord, prog?: IProgress): Promise<this> {
        this.loaded = false;
        this.stop();

        this.setTitle(data.title);

        arrayReplace(this.videoFormats, ...data.videos);
        arrayReplace(this.audioFormats, ...data.audios);

        const progs = progressSplit(prog, 4);
        await Promise.all([
            this.checkSources(this.audioFormats, progs.shift())
                .then(() => this.loadAudio(progs.shift())),
            this.checkSources(this.videoFormats, progs.shift())
                .then(() => this.loadVideo(progs.shift()))
        ]);

        this.loaded = true;
        this.dispatchEvent(this.loadEvt);
        return this;
    }

    private createMediaElement<T extends HTMLMediaElement>(MediaElement: (...rest: ElementChild[]) => T, ...rest: ElementChild[]): T {
        return MediaElement(
            playsInline(true),
            autoPlay(false),
            loop(false),
            ...rest
        );
    }

    private async loadMediaElement(elem: HTMLMediaElement, format: AudioRecord, prog?: IProgress): Promise<void> {
        elem.src = format.url;
        await mediaElementCanPlayThrough(elem, prog);;
    }

    private async loadAudio(prog?: IProgress): Promise<void> {
        if (this.audioFormats.length === 0) {
            throw new Error("No audio sources");
        }

        return await this.loadMediaElement(this.audio, this.audioFormats[0], prog);
    }

    private async loadVideo(prog?: IProgress): Promise<void> {
        if (this.videoFormats.length === 0) {
            throw new Error("No video sources");
        }

        return await this.loadMediaElement(this.video, this.videoFormats[0], prog);
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

