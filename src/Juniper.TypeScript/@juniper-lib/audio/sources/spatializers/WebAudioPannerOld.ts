import { BaseWebAudioPanner } from "./BaseWebAudioPanner";

/**
 * A positioner that uses the WebAudio API's old setPosition method.
 **/
export class WebAudioPannerOld extends BaseWebAudioPanner {

    /**
     * Creates a new positioner that uses the WebAudio API's old setPosition method.
     */
    constructor(id: string, audioCtx: AudioContext) {
        super(id, audioCtx);

        Object.seal(this);
    }

    override setPosition(x: number, y: number, z: number, _t: number) {
        this.panner.setPosition(x, y, z);
    }

    override setOrientation(x: number, y: number, z: number, _t: number) {
        this.panner.setOrientation(x, y, z);
    }
}
