import { BaseAudioElement } from "../BaseAudioElement";
import type { BaseEmitter } from "../sources/spatializers/BaseEmitter";
import type { BaseListener } from "./spatializers/BaseListener";
export declare type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;
export declare class AudioDestination extends BaseAudioElement<BaseListener, void> {
    private _trueDestination;
    private _remoteUserInput;
    private _spatializedInput;
    private _nonSpatializedInput;
    constructor(audioCtx: AudioContext, _trueDestination: DestinationNode);
    private disposed2;
    dispose(): void;
    get remoteUserInput(): AudioNode;
    get spatializedInput(): AudioNode;
    get nonSpatializedInput(): AudioNode;
    get trueDestination(): DestinationNode;
    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    createSpatializer(id: string, spatialize: boolean, isRemoteStream: boolean): BaseEmitter;
}
