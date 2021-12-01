import { BaseAudioSource } from "./BaseAudioSource";
export class AudioStreamSource extends BaseAudioSource {
    constructor(id, audioCtx, spatializer, ...effectNames) {
        super(id, audioCtx, spatializer, false, ...effectNames);
    }
}
