import { connect, Gain, removeVertex } from "../nodes";
import { BaseAudioElement } from "../BaseAudioElement";
import type { BaseEmitter } from "../sources/spatializers/BaseEmitter";
import type { BaseListener } from "./spatializers/BaseListener";
import { WebAudioListenerNew } from "./spatializers/WebAudioListenerNew";
import { WebAudioListenerOld } from "./spatializers/WebAudioListenerOld";

export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;

function createSpatializer(audioCtx: AudioContext): BaseListener {
    try {
        return new WebAudioListenerNew(audioCtx);
    }
    catch (exp) {
        console.warn("No AudioListener.positionX property!", exp);
        return new WebAudioListenerOld(audioCtx);
    }
}

export class AudioDestination extends BaseAudioElement<BaseListener, void> {
    private _remoteUserInput: AudioNode;
    private _spatializedInput: AudioNode;
    private _nonSpatializedInput: AudioNode;

    constructor(audioCtx: AudioContext, private _trueDestination: DestinationNode) {
        super("final", audioCtx, createSpatializer(audioCtx));

        this._remoteUserInput = Gain(
            "remote-user-input",
            this.audioCtx,
            null,
            this._spatializedInput = Gain(
                "spatialized-input",
                this.audioCtx,
                null,
                this.volumeControl));

        this._nonSpatializedInput = Gain(
            "non-spatialized-input",
            this.audioCtx,
            null,
            this.volumeControl);

        connect(this.volumeControl, this._trueDestination);
    }

    protected override onDisposing(): void {
        removeVertex(this._remoteUserInput);
        removeVertex(this._spatializedInput);
        removeVertex(this._nonSpatializedInput);
        super.onDisposing();
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
    createSpatializer(id: string, spatialize: boolean, isRemoteStream: boolean): BaseEmitter {
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
