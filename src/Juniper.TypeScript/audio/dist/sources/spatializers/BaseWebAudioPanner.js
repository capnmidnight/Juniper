import { Panner, removeVertex } from "../../nodes";
import { BaseEmitter } from "./BaseEmitter";
/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export class BaseWebAudioPanner extends BaseEmitter {
    panner;
    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(id, audioCtx) {
        super(id);
        this.input = this.output = this.panner = Panner(this.id, audioCtx, {
            panningModel: "HRTF",
            distanceModel: "inverse",
            coneInnerAngle: 360,
            coneOuterAngle: 0,
            coneOuterGain: 0
        });
    }
    disposed = false;
    dispose() {
        if (!this.disposed) {
            removeVertex(this.input);
            this.disposed = true;
        }
    }
    copyAudioProperties(from) {
        super.copyAudioProperties(from);
        this.panner.panningModel = from.panner.panningModel;
        this.panner.distanceModel = from.panner.distanceModel;
        this.panner.coneInnerAngle = from.panner.coneInnerAngle;
        this.panner.coneOuterAngle = from.panner.coneOuterAngle;
        this.panner.coneOuterGain = from.panner.coneOuterGain;
    }
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance, maxDistance, algorithm) {
        super.setAudioProperties(minDistance, maxDistance, algorithm);
        this.panner.refDistance = this.minDistance;
        this.panner.distanceModel = algorithm;
        if (this.maxDistance <= 0) {
            this.panner.rolloffFactor = Infinity;
        }
        else {
            this.panner.rolloffFactor = 1 / this.maxDistance;
        }
    }
    lpx = 0;
    lpy = 0;
    lpz = 0;
    lox = 0;
    loy = 0;
    loz = 0;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    update(loc, t) {
        const { p, f } = loc;
        const [px, py, pz] = p;
        const [ox, oy, oz] = f;
        if (px !== this.lpx
            || py !== this.lpy
            || pz !== this.lpz) {
            this.lpx = px;
            this.lpy = py;
            this.lpz = pz;
            this.setPosition(px, py, pz, t);
        }
        if (ox !== this.lox
            || oy !== this.loy
            || oz !== this.loz) {
            this.lox = ox;
            this.loy = oy;
            this.loz = oz;
            this.setOrientation(ox, oy, oz, t);
        }
    }
}
