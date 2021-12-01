import { TypedEvent } from "juniper-tslib";
import { ErsatzAudioNode } from "../nodes";
import { BaseAudioElement } from "../BaseAudioElement";
import type { BaseEmitter } from "./spatializers/BaseEmitter";
export declare class AudioSourceAddedEvent extends TypedEvent<"sourceadded"> {
    readonly source: AudioNode;
    constructor(source: AudioNode);
}
export interface AudioSourceEvents {
    "sourceadded": AudioSourceAddedEvent;
}
export declare abstract class BaseAudioSource<AudioNodeT extends AudioNode> extends BaseAudioElement<BaseEmitter, AudioSourceEvents> implements ErsatzAudioNode {
    readonly randomize: boolean;
    private source;
    private readonly delay;
    private readonly effects;
    constructor(id: string, audioCtx: AudioContext, spatializer: BaseEmitter, randomize: boolean, ...effectNames: string[]);
    private disposed2;
    dispose(): void;
    setEffects(...effectNames: string[]): void;
    get spatialized(): boolean;
    get input(): AudioNodeT;
    private get connectionPoint();
    protected connect(): void;
    protected disconnect(): void;
    set input(v: AudioNodeT);
    get output(): GainNode;
}
