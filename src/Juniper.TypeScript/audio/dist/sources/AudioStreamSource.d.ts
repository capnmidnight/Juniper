import { BaseAudioSource } from "./BaseAudioSource";
import type { BaseEmitter } from "./spatializers/BaseEmitter";
export declare type AudioStreamSourceNode = MediaElementAudioSourceNode | MediaStreamAudioSourceNode;
export declare class AudioStreamSource extends BaseAudioSource<AudioStreamSourceNode> {
    constructor(id: string, audioCtx: AudioContext, spatializer: BaseEmitter, ...effectNames: string[]);
}
