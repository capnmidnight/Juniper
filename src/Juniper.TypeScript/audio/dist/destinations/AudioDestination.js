import { connect, Gain, removeVertex } from "../nodes";
import { BaseAudioElement } from "../BaseAudioElement";
import { WebAudioListenerNew } from "./spatializers/WebAudioListenerNew";
import { WebAudioListenerOld } from "./spatializers/WebAudioListenerOld";
function createSpatializer(audioCtx) {
    try {
        return new WebAudioListenerNew(audioCtx);
    }
    catch (exp) {
        console.warn("No AudioListener.positionX property!", exp);
        return new WebAudioListenerOld(audioCtx);
    }
}
export class AudioDestination extends BaseAudioElement {
    _trueDestination;
    _remoteUserInput;
    _spatializedInput;
    _nonSpatializedInput;
    constructor(audioCtx, _trueDestination) {
        super("final", audioCtx, createSpatializer(audioCtx));
        this._trueDestination = _trueDestination;
        this._remoteUserInput = Gain("remote-user-input", this.audioCtx, null, this._spatializedInput = Gain("spatialized-input", this.audioCtx, null, this.volumeControl));
        this._nonSpatializedInput = Gain("non-spatialized-input", this.audioCtx, null, this.volumeControl);
        connect(this.volumeControl, this._trueDestination);
    }
    disposed2 = false;
    dispose() {
        if (!this.disposed2) {
            removeVertex(this._remoteUserInput);
            removeVertex(this._spatializedInput);
            removeVertex(this._nonSpatializedInput);
            this.disposed2 = true;
        }
        super.dispose();
    }
    get remoteUserInput() {
        return this._remoteUserInput;
    }
    get spatializedInput() {
        return this._spatializedInput;
    }
    get nonSpatializedInput() {
        return this._nonSpatializedInput;
    }
    get trueDestination() {
        return this._trueDestination;
    }
    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    createSpatializer(id, spatialize, isRemoteStream) {
        const destination = spatialize
            ? isRemoteStream
                ? this.remoteUserInput
                : this.spatializedInput
            : this.nonSpatializedInput;
        const spatializer = this.spatializer.createSpatializer(id, spatialize);
        connect(spatializer, destination);
        return spatializer;
    }
}
