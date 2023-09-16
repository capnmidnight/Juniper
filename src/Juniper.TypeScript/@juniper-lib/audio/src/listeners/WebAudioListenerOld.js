import { hasOldAudioListener } from "../util";
import { BaseWebAudioListener } from "./BaseWebAudioListener";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerOld extends BaseWebAudioListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(context) {
        if (!hasOldAudioListener) {
            throw new Error("WebAudio Listener API is not supported");
        }
        super("web-audio-listener-old", context);
        Object.seal(this);
    }
    setPosition(x, y, z) {
        this.listener.setPosition(x, y, z);
    }
    setOrientation(fx, fy, fz, ux, uy, uz) {
        this.listener.setOrientation(fx, fy, fz, ux, uy, uz);
    }
}
//# sourceMappingURL=WebAudioListenerOld.js.map