import { once, TypedEventBase } from "juniper-tslib";
import { IPlayable, MediaElementSourceEndedEvent, MediaElementSourceEvents, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "./IPlayable";

export class PlayableVideo
    extends TypedEventBase<MediaElementSourceEvents>
    implements IPlayable {
    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly endEvt: MediaElementSourceEndedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;

    constructor(private readonly video: HTMLVideoElement) {
        super();

        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.endEvt = new MediaElementSourceEndedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        const halt = (evt: Event) => {
            if (this.video.currentTime === 0) {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }

            if (evt.type === "ended") {
                this.dispatchEvent(this.endEvt);
            }
        };

        this.video.addEventListener("ended", halt);
        this.video.addEventListener("pause", halt);

        this.video.addEventListener("play", () =>
            this.dispatchEvent(this.playEvt));

        this.video.addEventListener("timeupdate", () => {
            this.progEvt.value = this.video.currentTime;
            this.progEvt.total = this.video.duration;
            this.dispatchEvent(this.progEvt);
        });
    }

    get width() {
        return this.video.videoWidth;
    }

    get height() {
        return this.video.videoHeight;
    }

    get playbackState(): PlaybackState {
        if (this.video.error) {
            return "errored";
        }

        if (this.video.ended || this.video.paused && this.video.currentTime === 0) {
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
        const endTask = once(this, "ended");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.video.pause();
    }

    stop(): void {
        this.video.currentTime = 0;
        this.pause();
    }

    restart(): void {
        this.stop();
        this.play();
    }
}
