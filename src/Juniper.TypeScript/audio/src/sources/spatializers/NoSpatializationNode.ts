import { Gain, removeVertex } from "../../nodes";
import type { Pose } from "../../Pose";
import { BaseEmitter } from "./BaseEmitter";


export class NoSpatializationNode extends BaseEmitter {

    /**
     * Creates a new "spatializer" that performs no panning. An anti-spatializer.
     */
    constructor(id: string, audioCtx: AudioContext) {
        super(id);
        this.input = this.output = Gain(this.id, audioCtx);
        Object.seal(this);
    }

    private disposed = false;
    override dispose(): void {
        if (!this.disposed) {
            removeVertex(this.input);
            this.disposed = true;
        }
    }

    update(_loc: Pose, _t: number): void {
        // do nothing
    }
}
