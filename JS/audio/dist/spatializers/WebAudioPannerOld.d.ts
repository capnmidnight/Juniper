import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseWebAudioPanner } from "./BaseWebAudioPanner";
/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export declare class WebAudioPannerOld extends BaseWebAudioPanner {
    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(context: JuniperAudioContext);
    setPosition(x: number, y: number, z: number): void;
    setOrientation(x: number, y: number, z: number): void;
}
//# sourceMappingURL=WebAudioPannerOld.d.ts.map