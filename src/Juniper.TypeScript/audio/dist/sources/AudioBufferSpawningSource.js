import { arrayClear, arrayRemove, once } from "juniper-tslib";
import { BufferSource, removeVertex } from "../nodes";
import { Pose } from "../Pose";
import { BaseAudioSource } from "./BaseAudioSource";
import { BaseEmitter } from "./spatializers/BaseEmitter";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";
class WrappedEmitter extends BaseEmitter {
    sub;
    constructor(sub) {
        super(`${sub.id}-wrapped`);
        this.sub = sub;
        this.input = this.sub.input;
        this.output = this.sub.output;
    }
    dispose() {
        // do nothing, someone else is disposing for us.
    }
    update(loc, t) {
        this.sub.update(loc, t);
    }
}
class AudioBufferSource extends BaseAudioSource {
    isPlaying = false;
    constructor(id, audioCtx, source, randomize, spatializer, ...effectNames) {
        super(id, audioCtx, spatializer, randomize, ...effectNames);
        this.input = source;
        this.disconnect();
    }
    disposed3 = false;
    dispose() {
        if (!this.disposed3) {
            this.stop();
            removeVertex(this.input);
            this.disposed3 = true;
        }
        super.dispose();
    }
    async play() {
        this.isPlaying = true;
        this.input.start();
        await once(this.input, "ended");
        this.stop();
    }
    stop() {
        this.input.stop();
        this.isPlaying = false;
    }
}
export class AudioBufferSpawningSource {
    id;
    audioCtx;
    source;
    randomize;
    spatializer;
    counter = 0;
    playingSources = new Array();
    _volume = 1;
    effectNames;
    constructor(id, audioCtx, source, randomize, spatializer, ...effectNames) {
        this.id = id;
        this.audioCtx = audioCtx;
        this.source = source;
        this.randomize = randomize;
        this.spatializer = spatializer;
        this.effectNames = effectNames;
    }
    disposed = false;
    dispose() {
        if (!this.disposed) {
            this.stop();
            this.spatializer.dispose();
            this.disposed = true;
        }
    }
    get volume() {
        return this._volume;
    }
    set volume(v) {
        if (v !== this._volume) {
            this._volume = v;
            for (const source of this.playingSources) {
                source.volume = v;
            }
        }
    }
    get spatialized() {
        return !(this.spatializer instanceof NoSpatializationNode);
    }
    pose = new Pose();
    get isPlaying() {
        for (const source of this.playingSources) {
            if (source.isPlaying) {
                return true;
            }
        }
        return false;
    }
    setEffects(...effectNames) {
        for (const source of this.playingSources) {
            source.setEffects(...effectNames);
        }
        this.effectNames = effectNames;
    }
    setAudioProperties(minDistance, maxDistance, algorithm) {
        this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
        for (const source of this.playingSources) {
            source.setAudioProperties(minDistance, maxDistance, algorithm);
        }
    }
    async play() {
        const newBuffer = BufferSource(`buffer-source-${this.id}`, this.audioCtx, {
            buffer: this.source.buffer,
            loop: this.source.loop
        });
        const newSource = new AudioBufferSource(`${this.id}-${this.counter++}`, this.audioCtx, newBuffer, this.randomize, new WrappedEmitter(this.spatializer), ...this.effectNames);
        this.playingSources.push(newSource);
        newSource.play();
        if (!this.source.loop) {
            await once(newBuffer, "ended");
            if (this.playingSources.indexOf(newSource) >= 0) {
                arrayRemove(this.playingSources, newSource);
                newSource.dispose();
            }
        }
    }
    stop() {
        for (const source of this.playingSources) {
            source.dispose();
        }
        arrayClear(this.playingSources);
    }
    update(t) {
        for (const source of this.playingSources) {
            source.pose.copy(this.pose);
            source.update(t);
        }
    }
}
