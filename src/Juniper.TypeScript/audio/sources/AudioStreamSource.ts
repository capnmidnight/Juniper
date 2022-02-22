import { BaseAudioSource } from "./BaseAudioSource";
import type { BaseEmitter } from "./spatializers/BaseEmitter";

export type AudioStreamSourceNode = MediaElementAudioSourceNode | MediaStreamAudioSourceNode;

export class AudioStreamSource extends BaseAudioSource<AudioStreamSourceNode> {
    constructor(id: string, audioCtx: AudioContext, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, ...effectNames);
    }
}
