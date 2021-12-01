import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export declare class WebAudioPannerOld extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(id: string, audioCtx: AudioContext);
    setPosition(x: number, y: number, z: number, _t: number): void;
    setOrientation(x: number, y: number, z: number, _t: number): void;
}
