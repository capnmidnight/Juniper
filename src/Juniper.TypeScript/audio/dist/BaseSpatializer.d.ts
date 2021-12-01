import type { Pose } from "./Pose";
/**
 * Base class providing functionality for spatializers.
 */
export declare abstract class BaseSpatializer {
    readonly id: string;
    minDistance: number;
    maxDistance: number;
    protected algorithm: DistanceModelType;
    constructor(id: string);
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract update(loc: Pose, t: number): void;
}
