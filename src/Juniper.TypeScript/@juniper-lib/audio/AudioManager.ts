import { autoPlay, id, playsInline, src, srcObject } from "@juniper-lib/dom/attrs";
import { display, styles } from "@juniper-lib/dom/css";
import { Audio, BackgroundAudio, elementApply, ErsatzElement, mediaElementCanPlay } from "@juniper-lib/dom/tags";
import {
    IDisposable, IProgress, isDefined,
    isMobileVR,
    isNullOrUndefined,
    isString, stringToName, TypedEvent, TypedEventBase
} from "@juniper-lib/tslib";
import type { DestinationNode } from "./destinations/AudioDestination";
import { AudioDestination } from "./destinations/AudioDestination";
import { WebAudioListenerNew } from "./destinations/spatializers/WebAudioListenerNew";
import { WebAudioListenerOld } from "./destinations/spatializers/WebAudioListenerOld";
import { canChangeAudioOutput, SpeakerManager } from "./SpeakerManager";
import type { IPoseable } from "./IPoseable";
import {
    audioReady,
    BiquadFilter, connect, disconnect,
    DynamicsCompressor, ErsatzAudioNode, Gain, hasNewAudioListener, MediaElementSource,
    MediaStreamDestination,
    MediaStreamSource
} from "./nodes";
import type { Pose } from "./Pose";
import { AudioElementSource } from "./sources/AudioElementSource";
import type { AudioStreamSourceNode } from "./sources/AudioStreamSource";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import { BaseEmitter } from "./sources/spatializers/BaseEmitter";
import { NoSpatializationNode } from "./sources/spatializers/NoSpatializationNode";
import { WebAudioPannerNew } from "./sources/spatializers/WebAudioPannerNew";
import { WebAudioPannerOld } from "./sources/spatializers/WebAudioPannerOld";

if (!("AudioContext" in globalThis) && "webkitAudioContext" in globalThis) {
    globalThis.AudioContext = (globalThis as any).webkitAudioContext;
}

if (!("OfflineAudioContext" in globalThis) && "webkitOfflineAudioContext" in globalThis) {
    globalThis.OfflineAudioContext = (globalThis as any).webkitOfflineAudioContext;
}


type withPoseCallback<T> = (pose: Pose) => T;


const USE_HEADPHONES_KEY = "juniper::useHeadphones";
const useHeadphonesToggledEvt = new TypedEvent("useheadphonestoggled");

const hasStreamSources = "createMediaStreamSource" in AudioContext.prototype;
const useElementSourceForUsers = !hasStreamSources;

interface AudioManagerEvents {
    useheadphonestoggled: TypedEvent<"useheadphonestoggled">;
}

/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export class AudioManager
    extends TypedEventBase<AudioManagerEvents>
    implements ErsatzElement, ErsatzAudioNode {

    private readonly users = new Map<string, AudioStreamSource>();
    private readonly clips = new Map<string, AudioElementSource>();
    private readonly clipPaths = new Map<string, string>();
    private readonly pathSources = new Map<string, Promise<MediaElementAudioSourceNode>>();
    private readonly pathCounts = new Map<string, number>();

    private readonly localFilter: BiquadFilterNode;
    private readonly localCompressor: DynamicsCompressorNode;

    private _minDistance = 1;
    private _maxDistance = 10;
    private _useHeadphones = false;

    private _algorithm: DistanceModelType = "inverse";
    get algorithm(): DistanceModelType { return this._algorithm; }

    readonly element: HTMLAudioElement = null;
    readonly audioDestination: AudioDestination = null;

    readonly speakers: SpeakerManager;
    readonly input: GainNode;
    readonly localAutoControlledGain: GainNode;
    readonly output: MediaStreamAudioDestinationNode;
    readonly audioCtx: AudioContext;
    readonly ready: Promise<void>;

    localUserID: string = null;

    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(defaultLocalUserID: string) {
        super();

        this.audioCtx = new AudioContext();

        let destination: DestinationNode = null;
        if (canChangeAudioOutput) {
            destination = MediaStreamDestination("final-destination", this.audioCtx);

            this.element = Audio(
                id("Audio-Device-Manager"),
                playsInline(true),
                autoPlay(true),
                srcObject(destination.stream),
                styles(
                    display("none")));

            elementApply(document.body, this);
        }
        else {
            destination = this.audioCtx.destination;
        }

        this.speakers = new SpeakerManager(this.element);

        this.input = Gain(
            "local-mic-user-gain",
            this.audioCtx,
            null,
            this.localAutoControlledGain = Gain(
                "local-mic-auto-gain",
                this.audioCtx,
                null,
                this.localFilter = BiquadFilter(
                    "local-mic-filter",
                    this.audioCtx,
                    {
                        type: "bandpass",
                        frequency: 1500,
                        Q: 0.25,
                    },
                    this.localCompressor = DynamicsCompressor(
                        "local-mic-compressor",
                        this.audioCtx, {
                        threshold: -15,
                        knee: 40,
                        ratio: 17
                    },
                        this.output = MediaStreamDestination(
                            "local-mic-destination",
                            this.audioCtx)))));

        this.audioDestination = new AudioDestination(this.audioCtx, destination, hasNewAudioListener
            ? new WebAudioListenerNew(this.audioCtx)
            : new WebAudioListenerOld(this.audioCtx));
        NoSpatializationNode.instance(this.audioCtx).setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);

        this.setLocalUserID(defaultLocalUserID);

        const useHeadphones = localStorage.getItem(USE_HEADPHONES_KEY);
        if (isDefined(useHeadphones)) {
            this._useHeadphones = useHeadphones === "true";
        }
        else {
            this._useHeadphones = isMobileVR();
        }

        this.ready = this.start();

        Object.seal(this);
    }

    get useHeadphones() {
        return this._useHeadphones;
    }

    set useHeadphones(v) {
        if (v !== this.useHeadphones) {
            this._useHeadphones = v;
            localStorage.setItem(USE_HEADPHONES_KEY, this.useHeadphones.toString());
            this.dispatchEvent(useHeadphonesToggledEvt);
        }
    }

    dispose() {
        for (const userID of this.users.keys()) {
            this.removeUser(userID);
        }

        for (const clipID of this.clips.keys()) {
            this.removeClip(clipID);
        }

        disconnect(this.input);
        disconnect(this.localAutoControlledGain);
        disconnect(this.localFilter);
        disconnect(this.localCompressor);
        disconnect(this.output);

        this.audioDestination.dispose();
        this.audioCtx.suspend();
    }

    private async start() {
        await audioReady(this.audioCtx);
        if (this.element) {
            await this.element.play();
        }
        await this.speakers.ready;
    }

    get filter() {
        return this.localFilter;
    }

    get filterFrequency() {
        return this.localFilter.frequency.value;
    }

    set filterFrequency(v) {
        this.localFilter.frequency.value = v;
    }

    get compressor() {
        return this.localCompressor;
    }

    /**
     * Gets the current playback time.
     */
    get currentTime(): number {
        return this.audioCtx.currentTime;
    }

    update(): void {
        const t = this.currentTime;
        this.audioDestination.audioTick(t);

        for (const clip of this.clips.values()) {
            clip.audioTick(t);
        }

        for (const user of this.users.values()) {
            user.audioTick(t);
        }
    }

    private counter = 0;

    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    newSpatializer(id: string, spatialize: boolean, isRemoteStream: boolean): BaseEmitter {
        if (spatialize) {
            const destination = isRemoteStream
                ? this.audioDestination.remoteUserInput
                : this.audioDestination.spatializedInput;

            const slug = `spatializer-${++this.counter}-${id}`;

            const spatializer = hasNewAudioListener
                ? new WebAudioPannerNew(slug + "-new-wa", this.audioCtx)
                : new WebAudioPannerOld(id + "-old-wa", this.audioCtx);

            connect(spatializer, destination);

            return spatializer;
        }
        else {
            return NoSpatializationNode.instance(this.audioCtx);
        }
    }

    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    private createSpatializer(id: string, spatialize: boolean, isRemoteStream: boolean): BaseEmitter {
        const spatializer = this.newSpatializer(id, spatialize, isRemoteStream);
        spatializer.setAudioProperties(this._minDistance, this._maxDistance, this._algorithm);
        return spatializer;
    }

    /**
     * Create a new user for audio processing.
     */
    createUser(userID: string, userName: string): AudioStreamSource {
        if (!this.users.has(userID)) {
            const id = stringToName(userName, userID);
            const spatializer = this.createSpatializer(id, true, true);
            const user = new AudioStreamSource(id, this.audioCtx, spatializer);
            this.users.set(userID, user);
        }

        return this.users.get(userID);
    }

    /**
     * Create a new user for the audio listener.
     */
    setLocalUserID(id: string): AudioDestination {
        if (this.audioDestination) {
            this.localUserID = id;
        }

        return this.audioDestination;
    }

    loadBasicClip(id: string, path: string, vol: number, prog?: IProgress): Promise<AudioElementSource> {
        return this.loadClip(id, path, false, false, false, false, vol, [], prog);
    }

    createBasicClip(id: string, element: HTMLMediaElement, vol: number): AudioElementSource {
        return this.createClip(id, element, false, false, false, vol, []);
    }

    /**
     * Creates a new sound effect from a series of fallback paths
     * for media files.
     * @param id - the name of the sound effect, to reference when executing playback.
     * @param looping - whether or not the sound effect should be played on loop.
     * @param autoPlaying - whether or not the sound effect should be played immediately.
     * @param spatialize - whether or not the sound effect should be spatialized.
     * @param vol - the volume at which to set the clip.
     * @param effectNames - names of pre-canned effects to load on the control.
     * @param path - a path for loading the media of the sound effect, or the sound effect that has already been loaded.
     * @param prog - an optional callback function to use for tracking progress of loading the clip.
     */
    async loadClip(
        id: string,
        path: string,
        looping: boolean,
        autoPlaying: boolean,
        spatialize: boolean,
        randomize: boolean,
        vol: number,
        effectNames: string[],
        prog?: IProgress): Promise<AudioElementSource> {
        if (isNullOrUndefined(path)
            || isString(path)
            && path.length === 0) {
            throw new Error("No clip source path provided");
        }

        if (isDefined(prog)) {
            prog.start(path);
        }

        const source = await this.getSourceTask(id, path, path, looping, autoPlaying, prog);
        const clip = this.makeClip(source, id, path, spatialize, randomize, autoPlaying, vol, ...effectNames);
        this.clips.set(id, clip);

        if (isDefined(prog)) {
            prog.end(path);
        }

        return clip;
    }

    createClip(
        id: string,
        element: HTMLMediaElement,
        autoPlaying: boolean,
        spatialize: boolean,
        randomize: boolean,
        vol: number,
        effectNames: string[]): AudioElementSource {
        if (isNullOrUndefined(element)
            || isString(element)
            && element.length === 0) {
            throw new Error("No clip source path provided");
        }

        const curPath = element.currentSrc;
        const source = this.createSourceFromElement(id, element);
        const clip = this.makeClip(source, id, curPath, spatialize, randomize, autoPlaying, vol, ...effectNames);
        this.clips.set(id, clip);

        return clip;
    }

    private getSourceTask(id: string, curPath: string, path: string, looping: boolean, autoPlaying: boolean, prog: IProgress): Promise<MediaElementAudioSourceNode> {
        this.clipPaths.set(id, curPath);
        let sourceTask = this.pathSources.get(curPath);
        if (isDefined(sourceTask)) {
            this.pathCounts.set(curPath, this.pathCounts.get(curPath) + 1);
        }
        else {
            sourceTask = this.createSourceFromFile(id, path, looping, autoPlaying, prog);
            this.pathSources.set(curPath, sourceTask);
            this.pathCounts.set(curPath, 1);
        }
        return sourceTask;
    }

    private async createSourceFromFile(id: string, path: string, looping: boolean, autoPlaying: boolean, prog?: IProgress): Promise<MediaElementAudioSourceNode> {
        if (isDefined(prog)) {
            prog.start(id);
        }

        const elem = BackgroundAudio(autoPlaying, false, looping, src(path));
        if (!await mediaElementCanPlay(elem)) {
            throw elem.error;
        }

        if (isDefined(prog)) {
            prog.end(id);
        }

        return this.createSourceFromElement(id, elem);
    }

    private createSourceFromStream(id: string, stream: MediaStream): AudioStreamSourceNode {
        const elem = BackgroundAudio(
            true,
            !useElementSourceForUsers,
            false,
            srcObject(stream));

        if (useElementSourceForUsers) {
            return this.createSourceFromElement(id, elem);
        }
        else {
            return MediaStreamSource(
                stringToName("media-stream-source", id, stream.id),
                this.audioCtx,
                stream);
        }
    }

    private createSourceFromElement(id: string, elem: HTMLMediaElement) {
        return MediaElementSource(
            stringToName("media-element-source", id),
            this.audioCtx,
            elem);
    }

    private makeClip(
        source: MediaElementAudioSourceNode,
        id: string,
        path: string,
        spatialize: boolean,
        randomize: boolean,
        autoPlaying: boolean,
        vol: number,
        ...effectNames: string[]): AudioElementSource {
        const nodeID = stringToName(id, path);
        const spatializer = this.createSpatializer(nodeID, spatialize, false);
        const clip = new AudioElementSource(
            stringToName("audio-clip-element", nodeID),
            this.audioCtx,
            source,
            randomize,
            spatializer,
            ...effectNames);

        if (autoPlaying) {
            clip.play();
        }

        clip.volume = vol;

        return clip;
    }

    hasClip(id: string): boolean {
        return this.clips.has(id);
    }

    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has started playing.
     * @param id - the name of the effect to play.
     */
    async playClip(id: string): Promise<void> {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            await clip.play();
        }
    }

    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has finished playing.
     * @param id - the name of the effect to play.
     */
    async playClipThrough(id: string): Promise<void> {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            await clip.playThrough();
        }
    }

    stopClip(id: string): void {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            clip.stop();
        }
    }

    /**
     * Get an existing user.
     */
    getUser(userID: string): AudioStreamSource {
        return this.users.get(userID);
    }

    /**
     * Get an existing audio clip.
     */
    getClip(id: string): AudioElementSource {
        return this.clips.get(id);
    }

    /**
     * Remove an audio source from audio processing.
     * @param sources - the collection of audio sources from which to remove.
     * @param id - the id of the audio source to remove
     **/
    private removeSource<T extends IDisposable>(sources: Map<string, T>, id: string): T {
        const source = sources.get(id);
        if (isDefined(source)) {
            sources.delete(id);
            source.dispose();
        }
        return source;
    }

    /**
     * Remove a user from audio processing.
     **/
    removeUser(userID: string): void {
        const user = this.removeSource(this.users, userID);
        if (isDefined(user.input)) {
            user.input = null;
        }
    }

    /**
     * Remove an audio clip from audio processing.
     **/
    removeClip(id: string): AudioElementSource {
        const path = this.clipPaths.get(id);
        this.pathCounts.set(path, this.pathCounts.get(path) - 1);
        if (this.pathCounts.get(path) === 0) {
            this.pathCounts.delete(path);
            this.pathSources.delete(path);
        }

        return this.removeSource(this.clips, id);
    }

    setUserStream(userID: string, userName: string, stream: MediaStream): void {
        if (this.users.has(userID)) {
            const user = this.users.get(userID);
            if (stream) {
                user.input = this.createSourceFromStream(
                    stringToName(userName, userID),
                    stream);
            }
            else if (isDefined(user.input)) {
                user.input = null;
            }
        }
    }

    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this._minDistance = minDistance;
        this._maxDistance = maxDistance;
        this._algorithm = algorithm;

        NoSpatializationNode.instance(this.audioCtx).setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);

        for (const user of this.users.values()) {
            user.spatializer.setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);
        }

        for (const clip of this.clips.values()) {
            clip.spatializer.setAudioProperties(clip.spatializer.minDistance, clip.spatializer.maxDistance, this.algorithm);
        }
    }

    /**
     * Get a pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param sources - the collection of poses from which to retrieve the pose.
     * @param id - the id of the pose for which to perform the operation.
     * @param poseCallback
     */
    private withPose<ElementT extends IPoseable, ResultT>(sources: Map<string, ElementT>, id: string, poseCallback: withPoseCallback<ResultT>): ResultT {
        const source = sources.get(id);
        let pose: Pose = null;
        if (source) {
            pose = source.pose;
        }
        else if (id === this.localUserID) {
            pose = this.audioDestination.pose;
        }

        if (!pose) {
            return null;
        }

        return poseCallback(pose);
    }

    /**
     * Get a user pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the user for which to perform the operation.
     * @param poseCallback
     */
    private withUser<T>(id: string, poseCallback: withPoseCallback<T>): T {
        return this.withPose(this.users, id, poseCallback);
    }

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
    setUserPose(id: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        this.withUser(id, (pose) => {
            pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        });
    }

    /**
     * Get an audio clip pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the audio clip for which to perform the operation.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     * @param poseCallback
     */
    private withClip<T>(id: string, poseCallback: withPoseCallback<T>): T {
        return this.withPose(this.clips, id, poseCallback);
    }

    /**
     * Set the position of an audio clip.
     * @param id - the id of the audio clip for which to set the position.
     * @param x - the horizontal component of the position.
     * @param y - the vertical component of the position.
     * @param z - the lateral component of the position.
     **/
    setClipPosition(id: string, x: number, y: number, z: number): void {
        this.withClip(id, (pose) => {
            pose.setPosition(x, y, z);
        });
    }

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
    setClipOrientation(id: string, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        this.withClip(id, (pose) => {
            pose.setOrientation(fx, fy, fz, ux, uy, uz);
        });
    }

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
    setClipPose(id: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        this.withClip(id, (pose) => {
            pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        });
    }
}