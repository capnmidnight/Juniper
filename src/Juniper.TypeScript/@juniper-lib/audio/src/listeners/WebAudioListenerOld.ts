import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { hasOldAudioListener } from "../util";
import { BaseWebAudioListener } from "./BaseWebAudioListener";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerOld extends BaseWebAudioListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(context: JuniperAudioContext) {
        if (!hasOldAudioListener) {
            throw new Error("WebAudio Listener API is not supported");
        }

        super("web-audio-listener-old", context);

        Object.seal(this);
    }

    protected setPosition(x: number, y: number, z: number): void {
        this.listener.setPosition(x, y, z);
    }

    protected setOrientation(
        fx: number, fy: number, fz: number,
        ux: number, uy: number, uz: number): void {
        this.listener.setOrientation(
            fx, fy, fz,
            ux, uy, uz);
    }
}