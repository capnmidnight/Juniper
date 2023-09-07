import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IAudioNode } from "../IAudioNode";
import { IPoseReader } from "../IPoseReader";
import { Pose } from "../Pose";
/**
 * Base class providing functionality for spatializers.
 */
export declare abstract class BaseSpatializer extends BaseNodeCluster implements IPoseReader {
    readonly spatialized: boolean;
    protected _minDistance: number;
    protected _maxDistance: number;
    protected _algorithm: DistanceModelType;
    constructor(type: string, context: JuniperAudioContext, spatialized: boolean, input?: ReadonlyArray<IAudioNode>, output?: ReadonlyArray<IAudioNode>, nodes?: ReadonlyArray<IAudioNode>);
    get minDistance(): number;
    get maxDistance(): number;
    get algorithm(): DistanceModelType;
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract readPose(loc: Pose): void;
    abstract getGainAtDistance(distance: number): number;
}
//# sourceMappingURL=BaseSpatializer.d.ts.map