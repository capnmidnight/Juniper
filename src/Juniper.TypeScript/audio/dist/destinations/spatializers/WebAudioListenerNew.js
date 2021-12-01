import { hasNewAudioListener } from "../../nodes";
import { WebAudioPannerNew } from "../../sources/spatializers/WebAudioPannerNew";
import { BaseListener } from "./BaseListener";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export class WebAudioListenerNew extends BaseListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(audioCtx) {
        super(audioCtx);
        if (!hasNewAudioListener) {
            throw new Error("Latest WebAudio Listener API is not supported");
        }
        Object.seal(this);
    }
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    update(loc, t) {
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
    newSpatializer(id) {
        return new WebAudioPannerNew(id, this.audioCtx);
    }
}
