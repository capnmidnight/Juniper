import { once, TypedEventBase } from "juniper-tslib";
import { IPlayable, PlaybackState, MediaElementSourceEvents, MediaElementSourcePlayedEvent, MediaElementSourcePausedEvent, MediaElementSourceStoppedEvent, MediaElementSourceProgressEvent } from "./IPlayable";

export class PlayableVideo
    extends TypedEventBase<MediaElementSourceEvents>
    implements IPlayable {
    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;

    constructor(private readonly video: HTMLVideoElement) {
        super();

        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.video.addEventListener("ended", () => {
            this.dispatchEvent(this.stopEvt);
        });

        this.video.addEventListener("timeupdate", () => {
            this.progEvt.value = this.video.currentTime;
            this.progEvt.total = this.video.duration;
            this.dispatchEvent(this.progEvt);
        });
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

    async play(): Promise<void> {
        if (this.playbackState !== "playing") {
            try {
                await this.video.play();
                this.dispatchEvent(this.playEvt);
            }
            catch (exp) {
                console.warn(exp);
            }

            if (!this.video.loop) {
                await once<AudioScheduledSourceNodeEventMap, "ended">(this.video, "ended");
                this.stop();
            }
        }
    }

    private halt() {
        if (this.playbackState === "playing") {
            this.video.pause();
        }
    }

    pause(): void {
        if (this.playbackState === "playing") {
            this.halt();
            this.dispatchEvent(this.pauseEvt);
        }
    }

    stop(): void {
        if (this.playbackState !== "stopped") {
            this.halt();
            this.video.currentTime = 0;
            this.dispatchEvent(this.stopEvt);
        }
    }

    restart(): void {
        if (this.playbackState !== "stopped") {
            this.stop();
            this.play();
        }
    }
}
