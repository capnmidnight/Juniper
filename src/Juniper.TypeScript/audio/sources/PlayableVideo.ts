import { ErsatzElement } from "juniper-dom/tags";
import { once, TypedEventBase } from "juniper-tslib";
import { IPlayable, MediaElementSourceEndedEvent, MediaElementSourceEvents, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "./IPlayable";

export class PlayableVideo
    extends TypedEventBase<MediaElementSourceEvents>
    implements ErsatzElement, IPlayable {

    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly endEvt: MediaElementSourceEndedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;

    constructor(public readonly element: HTMLVideoElement) {
        super();

        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.endEvt = new MediaElementSourceEndedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        const halt = (evt: Event) => {
            if (this.element.currentTime === 0) {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }

            if (evt.type === "ended") {
                this.dispatchEvent(this.endEvt);
            }
        };

        this.element.addEventListener("ended", halt);
        this.element.addEventListener("pause", halt);

        this.element.addEventListener("play", () =>
            this.dispatchEvent(this.playEvt));

        this.element.addEventListener("timeupdate", () => {
            this.progEvt.value = this.element.currentTime;
            this.progEvt.total = this.element.duration;
            this.dispatchEvent(this.progEvt);
        });
    }

    get width() {
        return this.element.videoWidth;
    }

    get height() {
        return this.element.videoHeight;
    }

    get playbackState(): PlaybackState {
        if (this.element.error) {
            return "errored";
        }

        if (this.element.ended || this.element.paused && this.element.currentTime === 0) {
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
        const endTask = once(this, "ended");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.element.pause();
    }

    stop(): void {
        this.element.currentTime = 0;
        this.pause();
    }

    restart(): void {
        this.stop();
        this.play();
    }
}
