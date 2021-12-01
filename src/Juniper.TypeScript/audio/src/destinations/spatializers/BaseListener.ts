import { BaseSpatializer } from "../../BaseSpatializer";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";
import { NoSpatializationNode } from "../../sources/spatializers/NoSpatializationNode";

/**
 * Base class providing functionality for audio listeners.
 **/
export abstract class BaseListener
    extends BaseSpatializer {

    private counter = 0;

    constructor(protected readonly audioCtx: AudioContext) {
        super("listener");
    }

    protected get listener(): AudioListener {
        return this.audioCtx.listener;
    }

    /**
     * Creates a spatialzer for an audio source.
     */
    createSpatializer(idPostfix: string, spatialize: boolean): BaseEmitter {
        const id = `spatializer-${this.counter++}-${idPostfix}`;
        if (spatialize) {
            return this.newSpatializer(id);
        }
        else {
            return new NoSpatializationNode(`no-${id}`, this.audioCtx);
        }
    }

    abstract newSpatializer(id: string): BaseEmitter;
}

