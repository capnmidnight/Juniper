import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseWebAudioListener } from "./BaseWebAudioListener";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export declare class WebAudioListenerOld extends BaseWebAudioListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(context: JuniperAudioContext);
    protected setPosition(x: number, y: number, z: number): void;
    protected setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
}
//# sourceMappingURL=WebAudioListenerOld.d.ts.map