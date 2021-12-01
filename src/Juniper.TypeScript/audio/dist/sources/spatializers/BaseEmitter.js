import { BaseSpatializer } from "../../BaseSpatializer";
/**
 * Base class providing functionality for audio listeners.
 **/
export class BaseEmitter extends BaseSpatializer {
    input;
    output;
    copyAudioProperties(from) {
        this.setAudioProperties(from.minDistance, from.maxDistance, from.algorithm);
    }
}
