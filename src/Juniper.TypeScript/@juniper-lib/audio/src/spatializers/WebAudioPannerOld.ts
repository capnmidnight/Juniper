import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseWebAudioPanner } from "./BaseWebAudioPanner";

/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export class WebAudioPannerOld extends BaseWebAudioPanner {

    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(context: JuniperAudioContext) {
        super("web-audio-panner-old", context);

        Object.seal(this);
    }

    override setPosition(x: number, y: number, z: number) {
        this.panner.setPosition(x, y, z);
    }

    override setOrientation(x: number, y: number, z: number) {
        this.panner.setOrientation(x, y, z);
    }
}