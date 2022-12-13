import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IAudioNode } from "../IAudioNode";
import { IPoseReader } from "../IPoseReader";
import { Pose } from "../Pose";


/**
 * Base class providing functionality for spatializers.
 */
export abstract class BaseSpatializer extends BaseNodeCluster implements IPoseReader {

    protected _minDistance = 1;
    protected _maxDistance = 10;
    protected _algorithm: DistanceModelType = "inverse";

    constructor(
        type: string,
        context: JuniperAudioContext,
        public readonly spatialized: boolean,
        input?: ReadonlyArray<IAudioNode>,
        output?: ReadonlyArray<IAudioNode>,
        nodes?: ReadonlyArray<IAudioNode>
    ) {
        super(type, context, input, output, nodes)
    }

    get minDistance() { return this._minDistance; }
    get maxDistance() { return this._maxDistance; }
    get algorithm() { return this._algorithm; }

    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this._minDistance = minDistance;
        this._maxDistance = maxDistance;
        this._algorithm = algorithm;
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract readPose(loc: Pose): void;

    abstract getGainAtDistance(distance: number): number;
}