import type { Pose } from "./Pose";

/**
 * Base class providing functionality for spatializers.
 */
export abstract class BaseSpatializer {

    constructor(public readonly id: string) {
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract setPose(loc: Pose, t: number): void;
}
