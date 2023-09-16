import { BaseNodeCluster } from "../BaseNodeCluster";
/**
 * Base class providing functionality for spatializers.
 */
export class BaseSpatializer extends BaseNodeCluster {
    constructor(type, context, spatialized, input, output, nodes) {
        super(type, context, input, output, nodes);
        this.spatialized = spatialized;
        this._minDistance = 1;
        this._maxDistance = 10;
        this._algorithm = "inverse";
    }
    get minDistance() { return this._minDistance; }
    get maxDistance() { return this._maxDistance; }
    get algorithm() { return this._algorithm; }
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance, maxDistance, algorithm) {
        this._minDistance = minDistance;
        this._maxDistance = maxDistance;
        this._algorithm = algorithm;
    }
}
//# sourceMappingURL=BaseSpatializer.js.map