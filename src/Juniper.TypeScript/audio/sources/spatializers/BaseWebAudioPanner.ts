import { Panner } from "../../nodes";
import { Pose } from "../../Pose";
import { BaseEmitter } from "./BaseEmitter";

/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export abstract class BaseWebAudioPanner extends BaseEmitter {

    protected readonly panner: PannerNode;

    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(id: string, audioCtx: AudioContext) {
        super(id);

        this.input = this.output = this.panner = Panner(this.id,
            audioCtx, {
            panningModel: "HRTF",
            distanceModel: "inverse",
            coneInnerAngle: 360,
            coneOuterAngle: 0,
            coneOuterGain: 0
        });
    }

    override copyAudioProperties(from: BaseWebAudioPanner) {
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
    override setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
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

    private lpx = 0;
    private lpy = 0;
    private lpz = 0;
    private lox = 0;
    private loy = 0;
    private loz = 0;

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    setPose(loc: Pose, t: number): void {
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

    protected abstract setPosition(x: number, y: number, z: number, t: number): void;
    protected abstract setOrientation(x: number, y: number, z: number, t: number): void;
}