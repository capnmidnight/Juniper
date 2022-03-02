import { once } from "juniper-tslib";
import { removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
import { IPlayable, MediaElementSourceEndedEvent, MediaElementSourceEvents, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent, PlaybackState } from "./IPlayable";
import type { BaseEmitter } from "./spatializers/BaseEmitter";

const elementRefCounts = new WeakMap<HTMLMediaElement, number>();
const nodeRefCounts = new WeakMap<MediaElementAudioSourceNode, number>();

function count(source: MediaElementAudioSourceNode, delta: number) {
    const nodeCount = (nodeRefCounts.get(source) || 0) + delta;
    nodeRefCounts.set(source, nodeCount);

    const elem = source.mediaElement;
    const elementCount = (elementRefCounts.get(elem) || 0) + delta;
    elementRefCounts.set(elem, elementCount);

    return elementCount;
}

function inc(source: MediaElementAudioSourceNode) {
    count(source, 1);
}

function dec(source: MediaElementAudioSourceNode) {
    count(source, -1);
    if (nodeRefCounts.get(source) === 0) {
        nodeRefCounts.delete(source);
        removeVertex(source);
    }

    if (elementRefCounts.get(source.mediaElement) === 0) {
        elementRefCounts.delete(source.mediaElement);
        if (source.mediaElement.isConnected) {
            source.mediaElement.remove();
        }
    }
}

export class AudioElementSource
    extends BaseAudioSource<MediaElementAudioSourceNode, MediaElementSourceEvents>
    implements IPlayable {
    private readonly playEvt: MediaElementSourcePlayedEvent;
    private readonly pauseEvt: MediaElementSourcePausedEvent;
    private readonly stopEvt: MediaElementSourceStoppedEvent;
    private readonly endEvt: MediaElementSourceEndedEvent;
    private readonly progEvt: MediaElementSourceProgressEvent;
    private readonly audio: HTMLAudioElement;

    constructor(id: string, audioCtx: AudioContext, source: MediaElementAudioSourceNode, private readonly randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, ...effectNames);
        inc(this.input = source);
        this.audio = source.mediaElement as HTMLAudioElement;
        this.disconnect();

        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.endEvt = new MediaElementSourceEndedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        const halt = (evt: Event) => {
            this.disconnect();

            if (this.audio.currentTime === 0) {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }

            if (evt.type === "ended") {
                this.dispatchEvent(this.endEvt);
            }
        };

        this.audio.addEventListener("ended", halt);
        this.audio.addEventListener("pause", halt);

        this.audio.addEventListener("play", () => {
            this.connect();

            if (this.randomize
                && this.audio.loop
                && this.audio.duration > 1) {
                const startTime = this.audio.duration * Math.random();
                this.audio.currentTime = startTime;
            }

            this.dispatchEvent(this.playEvt);
        });

        this.audio.addEventListener("timeupdate", () => {
            this.progEvt.value = this.audio.currentTime;
            this.progEvt.total = this.audio.duration;
            this.dispatchEvent(this.progEvt);
        });
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

    play(): Promise<void> {
        return this.audio.play();
    }

    async playThrough(): Promise<void> {
        const endTask = once(this, "ended");
        await this.play();
        await endTask;
    }

    pause(): void {
        this.audio.pause();
    }

    stop(): void {
        this.audio.currentTime = 0;
        this.pause();
    }

    restart(): void {
        this.stop();
        this.play();
    }

    protected override onDisposing(): void {
        this.disconnect();
        dec(this.input);
        super.onDisposing();
    }
}
