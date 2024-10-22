import { TypedEventMap } from "@juniper-lib/events";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { IAudioNode } from "../IAudioNode";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { IAudioSource } from "./IAudioSource";
export declare abstract class BaseAudioSource<EventTypeT extends TypedEventMap<string> = TypedEventMap<string>> extends BaseNodeCluster<EventTypeT> implements IAudioSource {
    readonly spatializer: BaseSpatializer;
    private readonly effects;
    protected readonly volumeControl: JuniperGainNode;
    private readonly pose;
    constructor(type: string, context: JuniperAudioContext, spatializer: BaseSpatializer, effectNames: string[], extras?: ReadonlyArray<IAudioNode>);
    protected onDisposing(): void;
    setEffects(...effectNames: string[]): void;
    get spatialized(): boolean;
    private get lastInternal();
    enable(): void;
    disable(): void;
    tog(): void;
    get volume(): number;
    set volume(v: number);
    get minDistance(): number;
    get maxDistance(): number;
    get algorithm(): DistanceModelType;
    setPosition(px: number, py: number, pz: number): void;
    setOrientation(qx: number, qy: number, qz: number, qw: number): void;
    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void;
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
}
//# sourceMappingURL=BaseAudioSource.d.ts.map