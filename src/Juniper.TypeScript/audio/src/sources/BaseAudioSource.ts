import { arrayClear, isDefined, isDisposable, TypedEvent } from "juniper-tslib";
import { AudioNodeType, chain, connect, Delay, disconnect, ErsatzAudioNode, removeVertex } from "../nodes";
import { BaseAudioElement } from "../BaseAudioElement";
import { effectStore } from "../effects";
import type { BaseEmitter } from "./spatializers/BaseEmitter";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";

export class AudioSourceAddedEvent extends TypedEvent<"sourceadded"> {
    constructor(public readonly source: AudioNode) {
        super("sourceadded");
    }
}

export interface AudioSourceEvents {
    "sourceadded": AudioSourceAddedEvent;
}

export abstract class BaseAudioSource<AudioNodeT extends AudioNode>
    extends BaseAudioElement<BaseEmitter, AudioSourceEvents>
    implements ErsatzAudioNode {

    private source: AudioNodeT = null;
    private readonly delay: DelayNode = null;
    private readonly effects = new Array<AudioNodeType>();

    constructor(id: string, audioCtx: AudioContext, spatializer: BaseEmitter, public readonly randomize: boolean, ...effectNames: string[]) {
        super(id, audioCtx, spatializer);

        if (this.randomize) {
            this.delay = Delay(
                `delay-${id}`,
                audioCtx, {
                delayTime: Math.random()
            },
                this.volumeControl);
        }

        this.setEffects(...effectNames);
    }

    private disposed2 = false;
    override dispose(): void {
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

    setEffects(...effectNames: string[]) {
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

        chain(
            this.volumeControl,
            ...this.effects,
            this.spatializer);
    }

    get spatialized() {
        return !(this.spatializer instanceof NoSpatializationNode);
    }

    get input() {
        return this.source;
    }

    private get connectionPoint() {
        return this.delay || this.volumeControl;
    }

    protected connect(): void {
        connect(this.source, this.connectionPoint);
    }

    protected disconnect(): void {
        disconnect(this.source, this.connectionPoint);
    }

    set input(v: AudioNodeT) {
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