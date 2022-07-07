import { isGoodNumber } from "@juniper-lib/tslib";
import { BaseWebAudioPanner } from "./BaseWebAudioPanner";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioPannerNew extends BaseWebAudioPanner {

    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(id: string, audioCtx: AudioContext) {
        super(id, audioCtx);

        Object.seal(this);
    }

    override setPosition(x: number, y: number, z: number, t: number) {
        if (isGoodNumber(x)
            && isGoodNumber(y)
            && isGoodNumber(z)
            && isGoodNumber(t)) {
            this.panner.positionX.setValueAtTime(x, t);
            this.panner.positionY.setValueAtTime(y, t);
            this.panner.positionZ.setValueAtTime(z, t);
        }
    }

    override setOrientation(x: number, y: number, z: number, t: number) {
        if (isGoodNumber(x)
            && isGoodNumber(y)
            && isGoodNumber(z)
            && isGoodNumber(t)) {
            this.panner.orientationX.setValueAtTime(-x, t);
            this.panner.orientationY.setValueAtTime(-y, t);
            this.panner.orientationZ.setValueAtTime(-z, t);
        }
    }
}

