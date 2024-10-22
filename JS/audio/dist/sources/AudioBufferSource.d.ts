import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { AudioSourceEvents } from "./AudioStreamSource";
import { BaseAudioSource } from "./BaseAudioSource";
export declare class AudioBufferSource extends BaseAudioSource<AudioSourceEvents> {
    private _buffer;
    private _node;
    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer, ...effectNames: string[]);
    get buffer(): AudioBuffer;
    set buffer(buffer: AudioBuffer);
}
//# sourceMappingURL=AudioBufferSource.d.ts.map