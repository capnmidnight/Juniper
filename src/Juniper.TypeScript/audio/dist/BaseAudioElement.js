import { isDisposable, TypedEventBase } from "juniper-tslib";
import { Gain, removeVertex } from "./nodes";
import { Pose } from "./Pose";
export class BaseAudioElement extends TypedEventBase {
    id;
    audioCtx;
    spatializer;
    pose = new Pose();
    volumeControl;
    constructor(id, audioCtx, spatializer) {
        super();
        this.id = id;
        this.audioCtx = audioCtx;
        this.spatializer = spatializer;
        this.volumeControl = Gain(`volume-control-${this.id}`, audioCtx);
    }
    get volume() {
        return this.volumeControl.gain.value;
    }
    set volume(v) {
        this.volumeControl.gain.value = v;
    }
    setAudioProperties(minDistance, maxDistance, algorithm) {
        this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
    }
    disposed = false;
    dispose() {
        if (!this.disposed) {
            removeVertex(this.volumeControl);
            if (isDisposable(this.spatializer)) {
                this.spatializer.dispose();
            }
            this.disposed = true;
        }
    }
    /**
     * Update the user.
     * @param t - the current update time.
     */
    update(t) {
        this.spatializer.update(this.pose, t);
    }
}
