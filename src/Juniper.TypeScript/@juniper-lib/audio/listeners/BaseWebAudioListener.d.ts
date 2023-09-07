import { Pose } from "../Pose";
import { BaseListener } from "./BaseListener";
export declare abstract class BaseWebAudioListener extends BaseListener {
    protected get listener(): AudioListener;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void;
    protected abstract setPosition(x: number, y: number, z: number): void;
    protected abstract setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
}
//# sourceMappingURL=BaseWebAudioListener.d.ts.map