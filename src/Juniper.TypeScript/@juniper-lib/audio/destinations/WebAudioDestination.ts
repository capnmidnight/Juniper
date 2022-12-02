import { BaseAudioElement } from "../BaseAudioElement";
import { Gain } from "../nodes";
import { NoSpatializationNode } from "../sources/spatializers/NoSpatializationNode";
import { connect, removeVertex } from "../util";
import type { BaseListener } from "./spatializers/BaseListener";

export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;

export class WebAudioDestination extends BaseAudioElement<BaseListener, void> {
    private _remoteUserInput: AudioNode;
    private _spatializedInput: AudioNode;
    private _nonSpatializedInput: AudioNode;

    constructor(audioCtx: AudioContext, private _trueDestination: DestinationNode, listener: BaseListener) {
        super("final", audioCtx, listener);

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
        connect(NoSpatializationNode.instance(this.audioCtx), this.nonSpatializedInput);
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
}
