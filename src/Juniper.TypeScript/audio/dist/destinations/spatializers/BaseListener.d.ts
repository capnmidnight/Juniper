import { BaseSpatializer } from "../../BaseSpatializer";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";
/**
 * Base class providing functionality for audio listeners.
 **/
export declare abstract class BaseListener extends BaseSpatializer {
    protected readonly audioCtx: AudioContext;
    private counter;
    constructor(audioCtx: AudioContext);
    protected get listener(): AudioListener;
    /**
     * Creates a spatialzer for an audio source.
     */
    createSpatializer(idPostfix: string, spatialize: boolean): BaseEmitter;
    abstract newSpatializer(id: string): BaseEmitter;
}
