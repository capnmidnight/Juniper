import { JuniperAudioContext } from "../context/JuniperAudioContext";
import type { Pose } from "../Pose";
import { hasNewAudioListener } from "../util";
import { BaseWebAudioListener } from "./BaseWebAudioListener";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerNew extends BaseWebAudioListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(context: JuniperAudioContext) {

        if (!hasNewAudioListener) {
            throw new Error("Latest WebAudio Listener API is not supported");
        }

        super("web-audio-listener-new", context);

        Object.seal(this);
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void {
        const { p, f, u } = loc;
        const t = this.context.currentTime;
        this.listener.positionX.setValueAtTime(p[0], t);
        this.listener.positionY.setValueAtTime(p[1], t);
        this.listener.positionZ.setValueAtTime(p[2], t);
        this.listener.forwardX.setValueAtTime(f[0], t);
        this.listener.forwardY.setValueAtTime(f[1], t);
        this.listener.forwardZ.setValueAtTime(f[2], t);
        this.listener.upX.setValueAtTime(u[0], t);
        this.listener.upY.setValueAtTime(u[1], t);
        this.listener.upZ.setValueAtTime(u[2], t);
    }
}
