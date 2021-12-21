import { once } from "juniper-tslib";
import { removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
import type { IPlayableSource } from "./IPlayableSource";
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
    extends BaseAudioSource<MediaElementAudioSourceNode>
    implements IPlayableSource {

    constructor(id: string, audioCtx: AudioContext, source: MediaElementAudioSourceNode, randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, randomize, ...effectNames);
        inc(this.input = source);
        this.disconnect();
    }

    private _isPlaying = false;
    get isPlaying(): boolean {
        return this._isPlaying;
    }

    set isPlaying(v: boolean) {
        if (v !== this.isPlaying) {
            this._isPlaying = v;
            if (this.isPlaying) {
                this.connect();
            }
            else {
                this.disconnect();
            }
        }
    }

    async play(): Promise<void> {
        this.isPlaying = true;
        try {
            await this.input.mediaElement.play();
        }
        catch (exp) {
            console.warn(exp);
        }

        if (!this.input.mediaElement.loop) {
            await once<AudioScheduledSourceNodeEventMap, "ended">(this.input.mediaElement, "ended");
            this.isPlaying = false;
        }
    }

    stop(): void {
        this.input.mediaElement.pause();
        this.input.mediaElement.currentTime = 0;
        this.isPlaying = false;
    }

    private disposed3 = false;
    override dispose(): void {
        if (!this.disposed3) {
            dec(this.input);
            this.disposed3 = false;
        }

        super.dispose();
    }
}
