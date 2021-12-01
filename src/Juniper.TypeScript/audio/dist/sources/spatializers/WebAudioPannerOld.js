import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export class WebAudioPannerOld extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(id, audioCtx) {
        super(id, audioCtx);
        Object.seal(this);
    }
    setPosition(x, y, z, _t) {
        this.panner.setPosition(x, y, z);
    }
    setOrientation(x, y, z, _t) {
        this.panner.setOrientation(x, y, z);
    }
}
