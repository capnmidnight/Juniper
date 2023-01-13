import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { mediaElementCanPlay } from "@juniper-lib/dom/tags";
import { once } from "@juniper-lib/tslib/events/once";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "../context/JuniperMediaElementAudioSourceNode";
import type { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";
import { IPlayable, MediaElementSourceErroredEvent, MediaElementSourceEvents, MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "./IPlayable";
import { PlaybackState } from "./PlaybackState";

export class AudioElementSource
    extends BaseAudioSource<MediaElementSourceEvents>
    implements IPlayable {

    private readonly loadEvt: MediaElementSourceLoadedEvent<IPlayable>;
    private readonly playEvt: MediaElementSourcePlayedEvent<IPlayable>;
    private readonly pauseEvt: MediaElementSourcePausedEvent<IPlayable>;
    private readonly stopEvt: MediaElementSourceStoppedEvent<IPlayable>;
    private readonly progEvt: MediaElementSourceProgressEvent<IPlayable>;
    readonly audio: HTMLMediaElement;

    constructor(
        context: JuniperAudioContext,
        source: JuniperMediaElementAudioSourceNode,
        randomizeStart: boolean,
        randomizePitch: boolean,
        spatializer: BaseSpatializer,
        ...effectNames: string[]) {

        super("audio-element-source", context, spatializer, effectNames);
        source.connect(this.volumeControl);

        this.audio = source.mediaElement;
        this.disable();

        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        const halt = (evt: Event) => {
            if (this.audio.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };

        this.audio.addEventListener("ended", halt);
        this.audio.addEventListener("pause", halt);

        if (randomizeStart) {
            this.audio.addEventListener("play", () => {
                if (this.audio.loop
                    && this.audio.duration > 1) {
                    const startTime = this.audio.duration * Math.random();
                    this.audio.currentTime = startTime;
                }
            });
        }

        if (randomizePitch) {
            source.mediaElement.preservesPitch = false;
            this.audio.addEventListener("play", () => {
                source.mediaElement.playbackRate = 1 + 0.1 * (2 * Math.random() - 1);
            });
        }

        this.audio.addEventListener("play", () => {
            this.dispatchEvent(this.playEvt);
        });

        this.audio.addEventListener("timeupdate", () => {
            this.progEvt.value = this.audio.currentTime;
            this.progEvt.total = this.audio.duration;
            this.dispatchEvent(this.progEvt);
        });

        if (this.audio.autoplay) {
            this.play()
                .catch(() =>
                    onUserGesture(() =>
                        this.play()));
        }

        mediaElementCanPlay(this.audio)
            .then((success) => this.dispatchEvent(success
                ? this.loadEvt
                : new MediaElementSourceErroredEvent(this, this.audio.error)));
    }

    get playbackState(): PlaybackState {
        if (this.audio.error) {
            return "errored";
        }

        if (this.audio.ended
            || this.audio.paused && this.audio.currentTime === 0) {
            return "stopped";
        }

        if (this.audio.paused) {
            return "paused";
        }

        return "playing";
    }

    async play(): Promise<void> {
        this.enable();
        await this.context.ready;
        await this.audio.play();
    }

    async playThrough(): Promise<void> {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.disable();
        this.audio.pause();
    }

    stop(): void {
        this.audio.currentTime = 0;
        this.pause();
    }

    restart(): Promise<void> {
        this.stop();
        return this.play();
    }
}
