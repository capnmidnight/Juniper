import { BaseSpatializer } from "./BaseSpatializer";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { Pose } from "../Pose";
/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export declare abstract class BaseWebAudioPanner extends BaseSpatializer {
    protected readonly panner: PannerNode;
    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(type: string, context: JuniperAudioContext);
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    private lpx;
    private lpy;
    private lpz;
    private lqx;
    private lqy;
    private lqz;
    private lqw;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void;
    /**
     * Computes an expected level of gain at a given distance based on the
     * algorithms expressed in the WebAudio API standard.
     * @param distance the distance to check
     * @returns the multiplicative gain that the panner node will end up applying to the audio signal
     * @see https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/distanceModel
     **/
    getGainAtDistance(distance: number): number;
    protected abstract setPosition(x: number, y: number, z: number, t?: number): void;
    protected abstract setOrientation(x: number, y: number, z: number, t?: number): void;
}
//# sourceMappingURL=BaseWebAudioPanner.d.ts.map