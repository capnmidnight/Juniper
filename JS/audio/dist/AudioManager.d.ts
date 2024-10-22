import { IReadyable, TypedEvent } from "@juniper-lib/events";
import { AssetFile, IFetcher } from "@juniper-lib/fetcher";
import { IProgress } from "@juniper-lib/progress";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { SpeakerManager } from "./SpeakerManager";
import { WebAudioDestination } from "./destinations/WebAudioDestination";
import { AudioElementSource } from "./sources/AudioElementSource";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import { BaseSpatializer } from "./spatializers/BaseSpatializer";
import { NoSpatializer } from "./spatializers/NoSpatializer";
type AudioManagerEvents = {
    useheadphonestoggled: TypedEvent<"useheadphonestoggled">;
};
export declare const RELEASE_EVT: TypedEvent<"released", EventTarget>;
/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export declare class AudioManager extends BaseNodeCluster<AudioManagerEvents> implements IReadyable {
    readonly fetcher: IFetcher;
    private readonly users;
    private readonly clips;
    private readonly pendingAudio;
    private readonly audioPool;
    private readonly elements;
    private readonly elementCounts;
    private _minDistance;
    private _maxDistance;
    private _algorithm;
    get algorithm(): DistanceModelType;
    private _useHeadphones;
    readonly destination: WebAudioDestination;
    readonly noSpatializer: NoSpatializer;
    readonly speakers: SpeakerManager;
    private readonly _ready;
    get ready(): Promise<void>;
    get isReady(): boolean;
    localUserID: string;
    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(fetcher: IFetcher, defaultLocalUserID: string);
    private enpool;
    preparePool(size: number): void;
    private getPooledAudio;
    private getPooledSource;
    private releasePooledSource;
    get useHeadphones(): boolean;
    set useHeadphones(v: boolean);
    protected onDisposing(): void;
    /**
     * Gets the current playback time.
     */
    get currentTime(): number;
    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether the audio stream is coming from a remote user.
     */
    createSpatializer(spatialize: boolean, isRemoteStream: boolean): BaseSpatializer;
    /**
     * Create a new user for audio processing.
     */
    createUser(userID: string, userName: string): AudioStreamSource;
    /**
     * Create a new user for the audio listener.
     */
    setLocalUserID(id: string): WebAudioDestination;
    createBasicClip(id: string, asset: AssetFile, vol: number): Promise<AudioElementSource>;
    hasClip(id: string): boolean;
    /**
     * Creates a new sound effect from a series of fallback paths
     * for media files.
     * @param id - the name of the sound effect, to reference when executing playback.
     * @param asset - the element to register as a clip
     * @param looping - whether the sound effect should be played on loop.
     * @param spatialize - whether the sound effect should be spatialized.
     * @param randomizeStart - whether the looping sound effect should be started somewhere in the middle.
     * @param randomizePitch - whether the sound effect should be pitch-bent whenever it is played.
     * @param vol - the volume at which to set the clip.
     * @param effectNames - names of pre-canned effects to load on the control.
     * @param path - a path for loading the media of the sound effect, or the sound effect that has already been loaded.
     * @param prog - an optional callback function to use for tracking progress of loading the clip.
     */
    createClip(id: string, asset: AssetFile | string, looping: boolean, spatialize: boolean, randomizeStart: boolean, randomizePitch: boolean, vol: number, effectNames: string[], prog?: IProgress): Promise<AudioElementSource>;
    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has started playing.
     * @param id - the name of the effect to play.
     */
    playClip(id: string): Promise<void>;
    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has finished playing.
     * @param id - the name of the effect to play.
     */
    playClipThrough(id: string): Promise<void>;
    stopClip(id: string): void;
    /**
     * Get an existing user.
     */
    getUser(userID: string): AudioStreamSource;
    /**
     * Get an existing audio clip.
     */
    getClip(id: string): AudioElementSource;
    /**
     * Remove an audio source from audio processing.
     * @param sources - the collection of audio sources from which to remove.
     * @param id - the id of the audio source to remove
     **/
    private removeSource;
    /**
     * Remove a user from audio processing.
     **/
    removeUser(userID: string): void;
    /**
     * Remove an audio clip from audio processing.
     **/
    removeClip(id: string): AudioElementSource;
    setUserStream(userID: string, stream: MediaStream): void;
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    /**
     * Get a pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param sources - the collection of poses from which to retrieve the pose.
     * @param id - the id of the pose for which to perform the operation.
     * @param poseCallback
     */
    private withPoser;
    /**
     * Get a user pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the user for which to perform the operation.
     * @param poseCallback
     */
    private withUser;
    /**
     * Get an audio clip pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the audio clip for which to perform the operation.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     * @param poseCallback
     */
    private withClip;
    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setUserPose(id: string, px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void;
    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     **/
    setUserPosition(id: string, px: number, py: number, pz: number): void;
    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setUserOrientation(id: string, qx: number, qy: number, qz: number, qw: number): void;
    /**
     * Set the position and orientation of an audio clip.
     * @param id - the id of the audio clip for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setClipPose(id: string, px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void;
    /**
     * Set the position and orientation of a clip.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     **/
    setClipPosition(id: string, px: number, py: number, pz: number): void;
    /**
     * Set the position and orientation of a clip.
     * @param id - the id of the user for which to set the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setClipOrientation(id: string, qx: number, qy: number, qz: number, qw: number): void;
}
export {};
//# sourceMappingURL=AudioManager.d.ts.map