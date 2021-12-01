import type { vec3 } from "gl-matrix";
import type { ErsatzElement } from "juniper-dom";
import type { IFetcher } from "juniper-fetcher";
import type { IProgress } from "juniper-tslib";
import { TypedEvent, TypedEventBase } from "juniper-tslib";
import type { ErsatzAudioNode } from "./nodes";
import { AudioDestination } from "./destinations/AudioDestination";
import { DeviceManager } from "./DeviceManager";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import type { IPlayableSource } from "./sources/IPlayableSource";
interface AudioManagerEvents {
    useheadphonestoggled: TypedEvent<"useheadphonestoggled">;
}
/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export declare class AudioManager extends TypedEventBase<AudioManagerEvents> implements ErsatzElement, ErsatzAudioNode {
    private readonly fetcher;
    private _minDistance;
    private _maxDistance;
    private _algorithm;
    get algorithm(): DistanceModelType;
    private _offsetRadius;
    private _useHeadphones;
    private readonly sortedUserIDs;
    private readonly users;
    private readonly clips;
    private readonly clipPaths;
    private readonly pathSources;
    private readonly pathCounts;
    readonly element: HTMLAudioElement;
    readonly devices: DeviceManager;
    localUserID: string;
    readonly input: GainNode;
    readonly localAutoControlledGain: GainNode;
    private readonly localFilter;
    private readonly localCompressor;
    readonly output: MediaStreamAudioDestinationNode;
    readonly audioDestination: AudioDestination;
    readonly audioCtx: AudioContext;
    readonly ready: Promise<void>;
    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(fetcher: IFetcher, defaultLocalUserID: string);
    get useHeadphones(): boolean;
    set useHeadphones(v: boolean);
    dispose(): void;
    private start;
    get offsetRadius(): number;
    set offsetRadius(v: number);
    get filter(): BiquadFilterNode;
    get filterFrequency(): number;
    set filterFrequency(v: number);
    get compressor(): DynamicsCompressorNode;
    get isReady(): boolean;
    /**
     * Gets the current playback time.
     */
    get currentTime(): number;
    update(): void;
    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    private createSpatializer;
    /**
     * Create a new user for audio processing.
     */
    createUser(userID: string, userName: string): AudioStreamSource;
    /**
     * Create a new user for the audio listener.
     */
    setLocalUserID(id: string): AudioDestination;
    createBasicClip(id: string, path: string, vol: number, onProgress?: IProgress): Promise<IPlayableSource>;
    /**
     * Creates a new sound effect from a series of fallback paths
     * for media files.
     * @param id - the name of the sound effect, to reference when executing playback.
     * @param looping - whether or not the sound effect should be played on loop.
     * @param autoPlaying - whether or not the sound effect should be played immediately.
     * @param spatialize - whether or not the sound effect should be spatialized.
     * @param vol - the volume at which to set the clip.
     * @param effectNames - names of pre-canned effects to load on the control.
     * @param path - a path for loading the media of the sound effect.
     * @param onProgress - an optional callback function to use for tracking progress of loading the clip.
     */
    createClip(id: string, path: string, looping: boolean, autoPlaying: boolean, spatialize: boolean, randomize: boolean, vol: number, effectNames: string[], onProgress?: IProgress): Promise<IPlayableSource>;
    private getSourceTask;
    private createSourceFromFile;
    private createSourceFromStream;
    private makeClip;
    hasClip(id: string): boolean;
    /**
     * Plays a named sound effect.
     * @param id - the name of the effect to play.
     */
    playClip(id: string): Promise<void>;
    stopClip(id: string): void;
    /**
     * Get an existing user.
     */
    getUser(userID: string): AudioStreamSource;
    /**
     * Get an existing audio clip.
     */
    getClip(id: string): IPlayableSource;
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
    removeClip(id: string): IPlayableSource;
    setUserStream(userID: string, userName: string, stream: MediaStream): void;
    updateUserOffsets(): void;
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
    private withPose;
    /**
     * Get a user pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the user for which to perform the operation.
     * @param poseCallback
     */
    private withUser;
    /**
     * Set the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     * @param x - the horizontal component of the offset.
     * @param y - the vertical component of the offset.
     * @param z - the lateral component of the offset.
     */
    private setUserOffset;
    /**
     * Get the comfort position offset for a given user.
     * @param id - the id of the user for which to set the offset.
     */
    getUserOffset(id: string): vec3;
    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     **/
    setUserPose(id: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    /**
     * Get an audio clip pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the audio clip for which to perform the operation.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     * @param poseCallback
     */
    private withClip;
    /**
     * Set the position of an audio clip.
     * @param id - the id of the audio clip for which to set the position.
     * @param x - the horizontal component of the position.
     * @param y - the vertical component of the position.
     * @param z - the lateral component of the position.
     **/
    setClipPosition(id: string, x: number, y: number, z: number): void;
    /**
     * Set the orientation of an audio clip.
     * @param id - the id of the audio clip for which to set the position.
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     **/
    setClipOrientation(id: string, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    /**
     * Set the position and orientation of an audio clip.
     * @param id - the id of the audio clip for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     **/
    setClipPose(id: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
}
export {};
