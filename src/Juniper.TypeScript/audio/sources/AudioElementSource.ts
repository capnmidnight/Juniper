import { once, TypedEvent } from "juniper-tslib";
import { removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
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

class AudioElementSourceEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly source: AudioElementSource) {
        super(type);
    }
}

class AudioElementSourcePlayedEvent extends AudioElementSourceEvent<"played"> {
    constructor(source: AudioElementSource) {
        super("played", source);
    }
}

class AudioElementSourcePausedEvent extends AudioElementSourceEvent<"paused"> {
    constructor(source: AudioElementSource) {
        super("paused", source);
    }
}

class AudioElementSourceStoppedEvent extends AudioElementSourceEvent<"stopped"> {
    constructor(source: AudioElementSource) {
        super("stopped", source);
    }
}

interface AudioElementSourceEvents {
    played: AudioElementSourcePlayedEvent;
    paused: AudioElementSourcePausedEvent;
    stopped: AudioElementSourceStoppedEvent;
}

type PlaybackState = "playing" | "paused" | "stopped";

export class AudioElementSource extends BaseAudioSource<MediaElementAudioSourceNode, AudioElementSourceEvents> {
    private readonly playEvt: AudioElementSourcePlayedEvent;
    private readonly pauseEvt: AudioElementSourcePausedEvent;
    private readonly stopEvt: AudioElementSourceStoppedEvent;

    constructor(id: string, audioCtx: AudioContext, source: MediaElementAudioSourceNode, randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, randomize, ...effectNames);
        inc(this.input = source);
        this.disconnect();

        this.playEvt = new AudioElementSourcePlayedEvent(this);
        this.pauseEvt = new AudioElementSourcePausedEvent(this);
        this.stopEvt = new AudioElementSourceStoppedEvent(this);
    }

    private _playbackState: PlaybackState = "stopped";
    get playbackState(): PlaybackState {
        return this._playbackState;
    }

    async play(): Promise<void> {
        if (this.playbackState !== "playing") {
            try {
                const playTask = this.input.mediaElement.play();
                this.connect();
                this._playbackState = "playing";
                this.dispatchEvent(this.playEvt);
                await playTask;
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
            this._playbackState = "paused";
            this.dispatchEvent(this.pauseEvt);
        }
    }

    stop(): void {
        if (this.playbackState !== "stopped") {
            this.halt();
            this.input.mediaElement.currentTime = 0;
            this._playbackState = "stopped";
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
