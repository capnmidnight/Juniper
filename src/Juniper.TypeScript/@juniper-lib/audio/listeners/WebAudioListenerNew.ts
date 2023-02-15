import { JuniperAudioContext } from "../context/JuniperAudioContext";
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

    protected setPosition(x: number, y: number, z: number): void {
        const t = this.context.currentTime;
        this.listener.positionX.setValueAtTime(x, t);
        this.listener.positionY.setValueAtTime(y, t);
        this.listener.positionZ.setValueAtTime(z, t);
    }

    protected setOrientation(
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number): void {
        const t = this.context.currentTime;
        this.listener.forwardX.setValueAtTime(fx, t);
        this.listener.forwardY.setValueAtTime(fy, t);
        this.listener.forwardZ.setValueAtTime(fz, t);
        this.listener.upX.setValueAtTime(ux, t);
        this.listener.upY.setValueAtTime(uy, t);
        this.listener.upZ.setValueAtTime(uz, t);
    }
}
