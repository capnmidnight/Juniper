import { arrayClear, isDefined, isDisposable, TypedEvent } from "juniper-tslib";
import { BaseAudioElement } from "../BaseAudioElement";
import { effectStore } from "../effects";
import { AudioNodeType, chain, connect, disconnect, ErsatzAudioNode, removeVertex } from "../nodes";
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

export abstract class BaseAudioSource<AudioNodeT extends AudioNode, EventTypeT = void>
    extends BaseAudioElement<BaseEmitter, AudioSourceEvents & EventTypeT>
    implements ErsatzAudioNode {

    private source: AudioNodeT = null;
    private readonly effects = new Array<AudioNodeType>();

    constructor(id: string, audioCtx: AudioContext, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer);

        this.setEffects(...effectNames);
    }

    protected override onDisposing(): void {
        for (const effect of this.effects) {
            if (isDisposable(effect)) {
                effect.dispose();
            }
        }

        arrayClear(this.effects);

        super.onDisposing();
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

    protected connect(): void {
        connect(this.source, this.volumeControl);
    }

    protected disconnect(): void {
        disconnect(this.source, this.volumeControl);
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