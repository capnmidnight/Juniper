import type { Pose } from "./Pose";

/**
 * Base class providing functionality for spatializers.
 */
export abstract class BaseSpatializer {

    minDistance = 1;
    maxDistance = 10;
    protected algorithm: DistanceModelType = "inverse";

    constructor(public readonly id: string) {
    }

    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        this.algorithm = algorithm;
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract setPose(loc: Pose, t: number): void;
}
