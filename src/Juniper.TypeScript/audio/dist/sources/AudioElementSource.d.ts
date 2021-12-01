import { BaseAudioSource } from "./BaseAudioSource";
import type { IPlayableSource } from "./IPlayableSource";
import type { BaseEmitter } from "./spatializers/BaseEmitter";
export declare class AudioElementSource extends BaseAudioSource<MediaElementAudioSourceNode> implements IPlayableSource {
    constructor(id: string, audioCtx: AudioContext, source: MediaElementAudioSourceNode, randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]);
    private _isPlaying;
    get isPlaying(): boolean;
    set isPlaying(v: boolean);
    play(): Promise<void>;
    stop(): void;
    private disposed3;
    dispose(): void;
}
