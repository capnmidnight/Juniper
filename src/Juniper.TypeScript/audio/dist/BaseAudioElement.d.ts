import { IDisposable, TypedEventBase } from "juniper-tslib";
import type { BaseSpatializer } from "./BaseSpatializer";
import type { IPoseable } from "./IPoseable";
import { Pose } from "./Pose";
export interface AudioElement extends IDisposable, IPoseable {
    volume: number;
    update(t: number): void;
}
export declare abstract class BaseAudioElement<SpatializerT extends BaseSpatializer, EventT> extends TypedEventBase<EventT> implements AudioElement {
    id: string;
    protected readonly audioCtx: AudioContext;
    readonly spatializer: SpatializerT;
    readonly pose: Pose;
    readonly volumeControl: GainNode;
    constructor(id: string, audioCtx: AudioContext, spatializer: SpatializerT);
    get volume(): number;
    set volume(v: number);
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    private disposed;
    dispose(): void;
    /**
     * Update the user.
     * @param t - the current update time.
     */
    update(t: number): void;
}
