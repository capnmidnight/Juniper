import { arrayClear, isDefined, isDisposable, TypedEvent } from "juniper-tslib";
import { chain, connect, Delay, disconnect, removeVertex } from "../nodes";
import { BaseAudioElement } from "../BaseAudioElement";
import { effectStore } from "../effects";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";
export class AudioSourceAddedEvent extends TypedEvent {
    source;
    constructor(source) {
        super("sourceadded");
        this.source = source;
    }
}
export class BaseAudioSource extends BaseAudioElement {
    randomize;
    source = null;
    delay = null;
    effects = new Array();
    constructor(id, audioCtx, spatializer, randomize, ...effectNames) {
        super(id, audioCtx, spatializer);
        this.randomize = randomize;
        if (this.randomize) {
            this.delay = Delay(`delay-${id}`, audioCtx, {
                delayTime: Math.random()
            }, this.volumeControl);
        }
        this.setEffects(...effectNames);
    }
    disposed2 = false;
    dispose() {
        if (!this.disposed2) {
            if (this.delay) {
                removeVertex(this.delay);
            }
            for (const effect of this.effects) {
                if (isDisposable(effect)) {
                    effect.dispose();
                }
            }
            arrayClear(this.effects);
            this.disposed2 = true;
        }
        super.dispose();
    }
    setEffects(...effectNames) {
        disconnect(this.volumeControl);
        for (const effect of this.effects) {
            if (isDisposable(effect)) {
                effect.dispose();
            }
        }
        arrayClear(this.effects);
        for (const effectName of effectNames) {
            if (isDefined(effectName)) {
                const effect = effectStore.get(effectName);
                if (isDefined(effect)) {
                    this.effects.push(effect(`${effectName}-${this.id}`, this.audioCtx));
                }
            }
        }
        chain(this.volumeControl, ...this.effects, this.spatializer);
    }
    get spatialized() {
        return !(this.spatializer instanceof NoSpatializationNode);
    }
    get input() {
        return this.source;
    }
    get connectionPoint() {
        return this.delay || this.volumeControl;
    }
    connect() {
        connect(this.source, this.connectionPoint);
    }
    disconnect() {
        disconnect(this.source, this.connectionPoint);
    }
    set input(v) {
        if (v !== this.input) {
            if (this.source) {
                removeVertex(this.source);
            }
            if (v) {
                this.source = v;
                this.connect();
                this.dispatchEvent(new AudioSourceAddedEvent(this.source));
            }
        }
    }
    get output() {
        return this.volumeControl;
    }
}
