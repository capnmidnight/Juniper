import { BaseSpatializer } from "./BaseSpatializer";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperPannerNode } from "../context/JuniperPannerNode";
import { Pose } from "../Pose";

/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export abstract class BaseWebAudioPanner extends BaseSpatializer {

    protected readonly panner: PannerNode;

    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(
        type: string,
        context: JuniperAudioContext) {
        const panner = new JuniperPannerNode(context, {
            panningModel: "HRTF",
            distanceModel: "inverse",
            coneInnerAngle: 360,
            coneOuterAngle: 0,
            coneOuterGain: 0
        });
        super(type, context, true, [panner]);

        this.panner = panner;
    }

    /**
     * Sets parameters that alter spatialization.
     **/
    override setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        super.setAudioProperties(minDistance, maxDistance, algorithm);
        this.panner.refDistance = this.minDistance;
        this.panner.distanceModel = algorithm;
        if (algorithm === "linear") {
            this.panner.rolloffFactor = 1;
        }
        else {
            if (this.maxDistance <= 0) {
                this.panner.rolloffFactor = Infinity;
            }
            else if (algorithm === "inverse") {
                this.panner.rolloffFactor = 1 / this.maxDistance;
            }
            else {
                this.panner.rolloffFactor = Math.pow(this.maxDistance, -0.2);
            }
        }
    }

    private lpx = 0;
    private lpy = 0;
    private lpz = 0;
    private lox = 0;
    private loy = 0;
    private loz = 0;

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void {
        const { p, f } = loc;
        const [px, py, pz] = p;
        const [ox, oy, oz] = f;
        if (px !== this.lpx
            || py !== this.lpy
            || pz !== this.lpz) {
            this.lpx = px;
            this.lpy = py;
            this.lpz = pz;
            this.setPosition(px, py, pz, this.context.currentTime);
        }

        if (ox !== this.lox
            || oy !== this.loy
            || oz !== this.loz) {
            this.lox = ox;
            this.loy = oy;
            this.loz = oz;
            this.setOrientation(ox, oy, oz, this.context.currentTime);
        }
    }

    /**
     * Computes an expected level of gain at a given distance based on the
     * algorithms expressed in the WebAudio API standard.
     * @param distance the distance to check
     * @returns the multiplicative gain that the panner node will end up applying to the audio signal
     * @see https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/distanceModel
     **/
    getGainAtDistance(distance: number) {
        const { rolloffFactor, refDistance, maxDistance, distanceModel } = this.panner;
        
        if (distance <= refDistance) {
            return 1;
        }
        else {
            const dDist = distance - refDistance;
            const dRef = maxDistance - refDistance;
            if (distanceModel === "linear") {
                return 1 - rolloffFactor * dDist / dRef;
            }
            else if (distanceModel === "inverse") {
                return refDistance / (refDistance + rolloffFactor * dDist);
            }
            else if (distanceModel === "exponential") {
                return Math.pow(distance / refDistance, -rolloffFactor);
            }
            else {
                assertNever(distanceModel);
            }
        }
    }

    protected abstract setPosition(x: number, y: number, z: number, t?: number): void;
    protected abstract setOrientation(x: number, y: number, z: number, t?: number): void;
}