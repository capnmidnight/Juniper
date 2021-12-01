import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export declare class WebAudioPannerNew extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(id: string, audioCtx: AudioContext);
    setPosition(x: number, y: number, z: number, t: number): void;
    setOrientation(x: number, y: number, z: number, t: number): void;
}
