import { hasOldAudioListener } from "../../nodes";
import type { Pose } from "../../Pose";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";
import { WebAudioPannerOld } from "../../sources/spatializers/WebAudioPannerOld";
import { BaseListener } from "./BaseListener";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerOld extends BaseListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(audioCtx: AudioContext) {
        super(audioCtx);

        if (!hasOldAudioListener) {
            throw new Error("WebAudio Listener API is not supported");
        }

        Object.seal(this);
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    update(loc: Pose, _t: number): void {
        const { p, f, u } = loc;
        this.listener.setPosition(p[0], p[1], p[2]);
        this.listener.setOrientation(f[0], f[1], f[2], u[0], u[1], u[2]);
    }

    /**
     * Creates a spatialzer for an audio source.
     */
    newSpatializer(id: string): BaseEmitter {
        return new WebAudioPannerOld(id, this.audioCtx);
    }
}