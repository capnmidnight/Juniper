import { BaseSpatializer } from "../../BaseSpatializer";

/**
 * Base class providing functionality for audio listeners.
 **/
export abstract class BaseListener
    extends BaseSpatializer {

    constructor(protected readonly audioCtx: AudioContext) {
        super("listener");
    }

    protected get listener(): AudioListener {
        return this.audioCtx.listener;
    }
}

