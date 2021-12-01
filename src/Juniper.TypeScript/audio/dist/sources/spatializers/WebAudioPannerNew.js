import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioPannerNew extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(id, audioCtx) {
        super(id, audioCtx);
        Object.seal(this);
    }
    setPosition(x, y, z, t) {
        this.panner.positionX.setValueAtTime(x, t);
        this.panner.positionY.setValueAtTime(y, t);
        this.panner.positionZ.setValueAtTime(z, t);
    }
    setOrientation(x, y, z, t) {
        this.panner.orientationX.setValueAtTime(-x, t);
        this.panner.orientationY.setValueAtTime(-y, t);
        this.panner.orientationZ.setValueAtTime(-z, t);
    }
}
