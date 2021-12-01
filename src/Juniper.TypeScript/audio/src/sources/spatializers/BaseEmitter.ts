import type { ErsatzAudioNode } from "../../nodes";
import { BaseSpatializer } from "../../BaseSpatializer";

/**
 * Base class providing functionality for audio listeners.
 **/
export abstract class BaseEmitter
    extends BaseSpatializer
    implements ErsatzAudioNode {

    input: AudioNode;
    output: AudioNode;

    abstract dispose(): void;

    protected copyAudioProperties(from: BaseEmitter) {
        this.setAudioProperties(
            from.minDistance,
            from.maxDistance,
            from.algorithm);
    }
}

