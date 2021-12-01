import { once } from "juniper-tslib";
import { removeVertex } from "../nodes";
import { BaseAudioSource } from "./BaseAudioSource";
const elementRefCounts = new WeakMap();
const nodeRefCounts = new WeakMap();
function count(source, delta) {
    const nodeCount = (nodeRefCounts.get(source) || 0) + delta;
    nodeRefCounts.set(source, nodeCount);
    const elem = source.mediaElement;
    const elementCount = (elementRefCounts.get(elem) || 0) + delta;
    elementRefCounts.set(elem, elementCount);
    return elementCount;
}
function inc(source) {
    count(source, 1);
}
function dec(source) {
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
export class AudioElementSource extends BaseAudioSource {
    constructor(id, audioCtx, source, randomize, spatializer, ...effectNames) {
        super(id, audioCtx, spatializer, randomize, ...effectNames);
        inc(this.input = source);
        this.disconnect();
    }
    _isPlaying = false;
    get isPlaying() {
        return this._isPlaying;
    }
    set isPlaying(v) {
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
    async play() {
        this.isPlaying = true;
        try {
            await this.input.mediaElement.play();
        }
        catch (exp) {
            console.warn(exp);
        }
        if (!this.input.mediaElement.loop) {
            await once(this.input.mediaElement, "ended");
            this.isPlaying = false;
        }
    }
    stop() {
        this.input.mediaElement.pause();
        this.input.mediaElement.currentTime = 0;
        this.isPlaying = false;
    }
    disposed3 = false;
    dispose() {
        if (!this.disposed3) {
            dec(this.input);
            this.disposed3 = false;
        }
        super.dispose();
    }
}
