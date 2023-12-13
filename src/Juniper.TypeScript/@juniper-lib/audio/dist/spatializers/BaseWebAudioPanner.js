import { BaseSpatializer } from "./BaseSpatializer";
import { JuniperPannerNode } from "../context/JuniperPannerNode";
import { assertNever } from "@juniper-lib/tslib/dist/typeChecks";
import { Vec3 } from "gl-matrix/dist/esm";
const fwd = new Vec3();
/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export class BaseWebAudioPanner extends BaseSpatializer {
    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(type, context) {
        const panner = new JuniperPannerNode(context, {
            panningModel: "HRTF",
            distanceModel: "inverse",
            coneInnerAngle: 360,
            coneOuterAngle: 0,
            coneOuterGain: 0
        });
        super(type, context, true, [panner]);
        this.lpx = 0;
        this.lpy = 0;
        this.lpz = 0;
        this.lqx = 0;
        this.lqy = 0;
        this.lqz = 0;
        this.lqw = 0;
        this.panner = panner;
    }
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance, maxDistance, algorithm) {
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
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc) {
        const { p, q } = loc;
        const [px, py, pz] = p;
        const [qx, qy, qz, qw] = q;
        if (px !== this.lpx
            || py !== this.lpy
            || pz !== this.lpz) {
            this.lpx = px;
            this.lpy = py;
            this.lpz = pz;
            this.setPosition(px, py, pz, this.context.currentTime);
        }
        if (qx !== this.lqx
            || qy !== this.lqy
            || qz !== this.lqz
            || qw !== this.lqw) {
            this.lqx = qx;
            this.lqy = qy;
            this.lqz = qz;
            this.lqw = qw;
            fwd.x = 0;
            fwd.y = 0;
            fwd.z = -1;
            Vec3.transformQuat(fwd, fwd, q);
            this.setOrientation(fwd[0], fwd[1], fwd[2], this.context.currentTime);
        }
    }
    /**
     * Computes an expected level of gain at a given distance based on the
     * algorithms expressed in the WebAudio API standard.
     * @param distance the distance to check
     * @returns the multiplicative gain that the panner node will end up applying to the audio signal
     * @see https://developer.mozilla.org/en-US/docs/Web/API/PannerNode/distanceModel
     **/
    getGainAtDistance(distance) {
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
}
//# sourceMappingURL=BaseWebAudioPanner.js.map