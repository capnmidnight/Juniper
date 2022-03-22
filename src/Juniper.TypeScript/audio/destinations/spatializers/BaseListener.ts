import { BaseSpatializer } from "../../BaseSpatializer";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";

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
    createSpatializer(idPostfix: string): BaseEmitter {
        return this.newSpatializer(`spatializer-${this.counter++}-${idPostfix}`);
    }

    protected abstract newSpatializer(id: string): BaseEmitter;
}

