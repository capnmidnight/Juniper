import { once } from "juniper-tslib";
import { removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
import { IPlayable, PlaybackState, MediaElementSourceEvents, MediaElementSourcePlayedEvent, MediaElementSourcePausedEvent, MediaElementSourceStoppedEvent, MediaElementSourceProgressEvent } from "./IPlayable";
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
    private readonly progEvt: MediaElementSourceProgressEvent;

    constructor(id: string, audioCtx: AudioContext, source: MediaElementAudioSourceNode, private readonly randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, ...effectNames);
        inc(this.input = source);
        this.disconnect();

        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);

        this.input.mediaElement.addEventListener("ended", () => {
            this.disconnect();
            this.dispatchEvent(this.pauseEvt);
        });

        this.input.mediaElement.addEventListener("timeupdate", () => {
            this.progEvt.value = this.input.mediaElement.currentTime;
            this.progEvt.total = this.input.mediaElement.duration;
            this.dispatchEvent(this.progEvt);
        });
    }

    get playbackState(): PlaybackState {
        const elem = this.input.mediaElement;
        if (elem.error) {
            return "errored";
        }

        if (elem.paused && elem.currentTime === 0) {
            return "stopped";
        }

        if (elem.paused || elem.ended) {
            return "paused";
        }

        return "playing";
    }

    async play(): Promise<void> {
        if (this.playbackState !== "playing") {
            try {
                let startTime = 0;
                if (this.randomize
                    && this.playbackState === "stopped"
                    && this.input.mediaElement.loop
                    && this.input.mediaElement.duration > 1) {
                    startTime = this.input.mediaElement.duration * Math.random();
                }
                const playTask = this.input.mediaElement.play();
                this.connect();
                await playTask;
                if (startTime > 0) {
                    this.input.mediaElement.currentTime = startTime;
                }
                this.dispatchEvent(this.playEvt);
            }
            catch (exp) {
                console.warn(exp);
            }

            if (!this.input.mediaElement.loop) {
                await once<AudioScheduledSourceNodeEventMap, "ended">(this.input.mediaElement, "ended");
                this.stop();
            }
        }
    }

    private halt() {
        if (this.playbackState === "playing") {
            this.disconnect();
            this.input.mediaElement.pause();
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
            this.input.mediaElement.currentTime = 0;
            this.dispatchEvent(this.stopEvt);
        }
    }

    restart(): void {
        if (this.playbackState !== "stopped") {
            this.stop();
            this.play();
        }
    }

    private disposed3 = false;
    override dispose(): void {
        if (!this.disposed3) {
            this.halt();
            dec(this.input);
            this.disposed3 = false;
        }

        super.dispose();
    }
}
