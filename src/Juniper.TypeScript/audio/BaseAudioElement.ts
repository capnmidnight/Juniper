import { IDisposable, isDisposable, TypedEventBase } from "juniper-tslib";
import { Gain, removeVertex } from "./nodes";
import type { BaseSpatializer } from "./BaseSpatializer";
import type { IPoseable } from "./IPoseable";
import { Pose } from "./Pose";

export interface AudioElement
    extends IDisposable, IPoseable {
    volume: number;
    update(t: number): void;
}

export abstract class BaseAudioElement<SpatializerT extends BaseSpatializer, EventT>
    extends TypedEventBase<EventT>
    implements AudioElement {
    readonly pose = new Pose();
    readonly volumeControl: GainNode;

    constructor(
        public id: string,
        protected readonly audioCtx: AudioContext,
        public readonly spatializer: SpatializerT) {
        super();

        this.volumeControl = Gain(`volume-control-${this.id}`, audioCtx);
    }

    get volume(): number {
        return this.volumeControl.gain.value;
    }

    set volume(v: number) {
        this.volumeControl.gain.value = v;
    }

    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            removeVertex(this.volumeControl);
            if (isDisposable(this.spatializer)) {
                this.spatializer.dispose();
            }
            this.disposed = true;
        }
    }

    /**
     * Update the user.
     * @param t - the current update time.
     */
    update(t: number): void {
        this.spatializer.update(this.pose, t);
    }
}