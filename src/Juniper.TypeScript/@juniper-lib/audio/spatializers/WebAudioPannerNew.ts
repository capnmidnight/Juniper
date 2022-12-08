import { isBadNumber, isGoodNumber } from "@juniper-lib/tslib/typeChecks";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseWebAudioPanner } from "./BaseWebAudioPanner";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioPannerNew extends BaseWebAudioPanner {

    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(context: JuniperAudioContext) {
        super("web-audio-panner-new", context);

        Object.seal(this);
    }

    setPosition(x: number, y: number, z: number, t: number) {
        if (isGoodNumber(x)
            && isGoodNumber(y)
            && isGoodNumber(z)) {
            if (isBadNumber(t)) {
                t = 0;
            }
            this.panner.positionX.setValueAtTime(x, t);
            this.panner.positionY.setValueAtTime(y, t);
            this.panner.positionZ.setValueAtTime(z, t);
        }
    }

    setOrientation(x: number, y: number, z: number, t: number) {
        if (isGoodNumber(x)
            && isGoodNumber(y)
            && isGoodNumber(z)) {
            if (isBadNumber(t)) {
                t = 0;
            }
            this.panner.orientationX.setValueAtTime(-x, t);
            this.panner.orientationY.setValueAtTime(-y, t);
            this.panner.orientationZ.setValueAtTime(-z, t);
        }
    }
}

