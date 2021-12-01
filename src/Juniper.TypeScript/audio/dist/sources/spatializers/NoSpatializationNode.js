import { Gain, removeVertex } from "../../nodes";
import { BaseEmitter } from "./BaseEmitter";
export class NoSpatializationNode extends BaseEmitter {
    /**
     * Creates a new "spatializer" that performs no panning. An anti-spatializer.
     */
    constructor(id, audioCtx) {
        super(id);
        this.input = this.output = Gain(this.id, audioCtx);
        Object.seal(this);
    }
    disposed = false;
    dispose() {
        if (!this.disposed) {
            removeVertex(this.input);
            this.disposed = true;
        }
    }
    update(_loc, _t) {
        // do nothing
    }
}
