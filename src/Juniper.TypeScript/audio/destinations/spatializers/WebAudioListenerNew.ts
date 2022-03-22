import { hasNewAudioListener } from "../../nodes";
import type { Pose } from "../../Pose";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";
import { WebAudioPannerNew } from "../../sources/spatializers/WebAudioPannerNew";
import { BaseListener } from "./BaseListener";

/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerNew extends BaseListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(audioCtx: AudioContext) {
        super(audioCtx);

        if (!hasNewAudioListener) {
            throw new Error("Latest WebAudio Listener API is not supported");
        }

        Object.seal(this);
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    setPose(loc: Pose, t: number): void {
        const { p, f, u } = loc;
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

    /**
     * Creates a spatialzer for an audio source.
     */
    protected newSpatializer(id: string): BaseEmitter {
        return new WebAudioPannerNew(id, this.audioCtx);
    }
}
