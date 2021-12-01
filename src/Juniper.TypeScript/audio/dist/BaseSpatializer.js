/**
 * Base class providing functionality for spatializers.
 */
export class BaseSpatializer {
    id;
    minDistance = 1;
    maxDistance = 10;
    algorithm = "inverse";
    constructor(id) {
        this.id = id;
    }
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance, maxDistance, algorithm) {
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        this.algorithm = algorithm;
    }
}
