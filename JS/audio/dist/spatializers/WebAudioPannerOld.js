import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export class WebAudioPannerOld extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(context) {
        super("web-audio-panner-old", context);
        Object.seal(this);
    }
    setPosition(x, y, z) {
        this.panner.setPosition(x, y, z);
    }
    setOrientation(x, y, z) {
        this.panner.setOrientation(x, y, z);
    }
}
//# sourceMappingURL=WebAudioPannerOld.js.map