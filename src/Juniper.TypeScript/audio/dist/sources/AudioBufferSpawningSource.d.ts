import { Pose } from "../Pose";
import type { IPlayableSource } from "./IPlayableSource";
import { BaseEmitter } from "./spatializers/BaseEmitter";
export declare class AudioBufferSpawningSource implements IPlayableSource {
    readonly id: string;
    private readonly audioCtx;
    private readonly source;
    private readonly randomize;
    private readonly spatializer;
    private counter;
    private playingSources;
    private _volume;
    private effectNames;
    constructor(id: string, audioCtx: AudioContext, source: AudioBufferSourceNode, randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]);
    private disposed;
    dispose(): void;
    get volume(): number;
    set volume(v: number);
    get spatialized(): boolean;
    readonly pose: Pose;
    get isPlaying(): boolean;
    setEffects(...effectNames: string[]): void;
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    play(): Promise<void>;
    stop(): void;
    update(t: number): void;
}
