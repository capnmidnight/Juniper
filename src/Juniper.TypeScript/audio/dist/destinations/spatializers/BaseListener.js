import { BaseSpatializer } from "../../BaseSpatializer";
import { NoSpatializationNode } from "../../sources/spatializers/NoSpatializationNode";
/**
 * Base class providing functionality for audio listeners.
 **/
export class BaseListener extends BaseSpatializer {
    audioCtx;
    counter = 0;
    constructor(audioCtx) {
        super("listener");
        this.audioCtx = audioCtx;
    }
    get listener() {
        return this.audioCtx.listener;
    }
    /**
     * Creates a spatialzer for an audio source.
     */
    createSpatializer(idPostfix, spatialize) {
        const id = `spatializer-${this.counter++}-${idPostfix}`;
        if (spatialize) {
            return this.newSpatializer(id);
        }
        else {
            return new NoSpatializationNode(`no-${id}`, this.audioCtx);
        }
    }
}
