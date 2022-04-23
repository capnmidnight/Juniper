import { IDisposable, isDisposable, TypedEventBase } from "@juniper/tslib";
import type { BaseSpatializer } from "./BaseSpatializer";
import type { IPoseable } from "./IPoseable";
import { Gain, removeVertex } from "./nodes";
import { Pose } from "./Pose";

export interface AudioElement
    extends IDisposable, IPoseable {
    volume: number;
    audioTick(t: number): void;
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

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            this.onDisposing();
            this.disposed = true;
        }
    }

    protected onDisposing(): void {
        removeVertex(this.volumeControl);
        if (isDisposable(this.spatializer)) {
            this.spatializer.dispose();
        }
    }

    /**
     * Update the user.
     */
    audioTick(t: number): void {
        this.spatializer.setPose(this.pose, t);
    }
}
