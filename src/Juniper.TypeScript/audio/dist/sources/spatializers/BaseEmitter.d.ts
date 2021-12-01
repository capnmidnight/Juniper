import type { ErsatzAudioNode } from "../../nodes";
import { BaseSpatializer } from "../../BaseSpatializer";
/**
 * Base class providing functionality for audio listeners.
 **/
export declare abstract class BaseEmitter extends BaseSpatializer implements ErsatzAudioNode {
    input: AudioNode;
    output: AudioNode;
    abstract dispose(): void;
    protected copyAudioProperties(from: BaseEmitter): void;
}
