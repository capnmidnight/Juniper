import { autoPlay, controls, loop, muted, src } from "@juniper-lib/dom/attrs";
import { onEvent } from "@juniper-lib/dom/evts";
import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { Audio, elementApply, ElementChild } from "@juniper-lib/dom/tags";
import { AssetFile } from "@juniper-lib/fetcher/Asset";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { unwrapResponse } from "@juniper-lib/fetcher/unwrapResponse";
import { arrayClear, arrayRemove } from "@juniper-lib/tslib/collections/arrays";
import { all } from "@juniper-lib/tslib/events/all";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { IReadyable } from "@juniper-lib/tslib/events/IReadyable";
import { Task } from "@juniper-lib/tslib/events/Task";
import { Exception } from "@juniper-lib/tslib/Exception";
import { isMobileVR } from "@juniper-lib/tslib/flags";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { stringToName } from "@juniper-lib/tslib/strings/stringToName";
import { isDefined, isString } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "./context/JuniperMediaElementAudioSourceNode";
import { WebAudioDestination } from "./destinations/WebAudioDestination";
import { IPoseable } from "./IPoseable";
import { AudioElementSource } from "./sources/AudioElementSource";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import type { IAudioSource } from "./sources/IAudioSource";
import { BaseSpatializer } from "./spatializers/BaseSpatializer";
import { NoSpatializer } from "./spatializers/NoSpatializer";
import { SpeakerManager } from "./SpeakerManager";

type withPoserCallback<T> = (source: IPoseable) => T;

const USE_HEADPHONES_KEY = "juniper::useHeadphones";
const useHeadphonesToggledEvt = new TypedEvent("useheadphonestoggled");

interface AudioManagerEvents {
    useheadphonestoggled: TypedEvent<"useheadphonestoggled">;
}

const POOL_SIZE = 20;
export const RELEASE_EVT = new TypedEvent("released");

// (This is a tiny MP3 file that is silent and extremely short - retrieved from https://bigsoundbank.com and then modified)
const HAX_SRC = "data:audio/mpeg;base64,SUQzBAAAAAABEVRYWFgAAAAtAAADY29tbWVudABCaWdTb3VuZEJhbmsuY29tIC8gTGFTb25vdGhlcXVlLm9yZwBURU5DAAAAHQAAA1N3aXRjaCBQbHVzIMKpIE5DSCBTb2Z0d2FyZQBUSVQyAAAABgAAAzIyMzUAVFNTRQAAAA8AAANMYXZmNTcuODMuMTAwAAAAAAAAAAAAAAD/80DEAAAAA0gAAAAATEFNRTMuMTAwVVVVVVVVVVVVVUxBTUUzLjEwMFVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVf/zQsRbAAADSAAAAABVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVf/zQMSkAAADSAAAAABVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV";

/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export class AudioManager
    extends BaseNodeCluster<AudioManagerEvents>
    implements IReadyable {

    private readonly users = new Map<string, AudioStreamSource>();
    private readonly clips = new Map<string, AudioElementSource>();
    private readonly clipPaths = new Map<string, string>();
    private readonly pathSources = new Map<string, Promise<JuniperMediaElementAudioSourceNode>>();
    private readonly pathCounts = new Map<string, number>();

    private readonly pendingAudio = new Array<() => void>();
    private readonly audioPool = new Array<Task<HTMLAudioElement>>();
    private readonly audioInUse = new Array<Task<HTMLAudioElement>>();

    private _minDistance = 0.25;
    private _maxDistance = 100;
    private _algorithm: DistanceModelType = "inverse";
    get algorithm(): DistanceModelType { return this._algorithm; }

    private _useHeadphones = false;


    get element() { return this.destination.element; }

    readonly destination: WebAudioDestination = null;
    readonly noSpatializer: NoSpatializer;
    readonly speakers: SpeakerManager;

    private readonly _ready = new Task();
    get ready(): Promise<void> {
        return this._ready;
    }

    get isReady() {
        return this._ready.finished
            && this._ready.resolved;
    }

    localUserID: string = null;

    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(public readonly fetcher: IFetcher, defaultLocalUserID: string) {
        const context = new JuniperAudioContext();

        if ("THREE" in globalThis) {
            globalThis.THREE.AudioContext.setContext(context);
        }

        const destination = new WebAudioDestination(context);

        const noSpatializer = new NoSpatializer(destination.nonSpatializedInput);

        const speakers = new SpeakerManager(destination.element);

        super("audio-manager", context, null, null, [noSpatializer, destination]);

        this.destination = destination;
        this.noSpatializer = noSpatializer;
        this.speakers = speakers;

        all(
            this.context.ready,
            this.destination.ready,
            this.speakers.ready
        ).then(() => this._ready.resolve());

        this.setLocalUserID(defaultLocalUserID);

        const useHeadphones = localStorage.getItem(USE_HEADPHONES_KEY);
        if (isDefined(useHeadphones)) {
            this._useHeadphones = useHeadphones === "true";
        }
        else {
            this._useHeadphones = isMobileVR();
        }

        this.enpool();

        onUserGesture(() => {
            const depooling = [...this.pendingAudio];
            arrayClear(this.pendingAudio);
            for (const p of depooling) {
                p();
                console.log("Depooled");
            }

        }, true);

        Object.seal(this);
    }

    private enpool() {
        console.log("Enpooling");
        for (let i = 0; i < POOL_SIZE; ++i) {
            const task = new Task<HTMLAudioElement>();
            const audio = Audio(
                src(HAX_SRC),
                controls(false),
                autoPlay(true),
                muted(true),
                onEvent("released", () => {
                    console.log("RELEASED!", audio);
                    arrayRemove(this.audioInUse, task);
                    this.audioPool.unshift(task);
                })
            );

            this.pendingAudio.push(() => {
                audio.pause();
                task.resolve(audio);
            });
            this.audioPool.push(task);
        }
    }


    private async getPooledAudio(...rest: ElementChild[]): Promise<HTMLAudioElement> {
        if (this.audioPool.length === 0) {
            throw new Exception("Audio pool exhausted!");
        }

        const audioTask = this.audioPool.shift();
        if (this.audioPool.length <= POOL_SIZE / 2) {
            this.enpool();
        }
        this.audioInUse.push(audioTask);

        const audio = await audioTask;

        elementApply(audio, ...rest);

        return audio;
    }

    private async getPooledSource(key: string, path: string, autoPlaying: boolean, looping: boolean): Promise<JuniperMediaElementAudioSourceNode> {
        if (!this.elements.has(key)) {
            const mediaElement = await this.getPooledAudio(
                muted(false),
                src(path),
                loop(looping),
                autoPlay(autoPlaying));
            const node = new JuniperMediaElementAudioSourceNode(
                this.context,
                { mediaElement });
            node.name = stringToName("media-element-source", key);
            this.elements.set(key, node);
            this.elementCounts.set(node, 0);
        }

        const source = this.elements.get(key);
        this.elementCounts.set(source, this.elementCounts.get(source) + 1);
        return source;
    }

    private releasePooledSource(key: string, source: JuniperMediaElementAudioSourceNode): void {
        this.elementCounts.set(source, this.elementCounts.get(source) - 1);
        if (this.elementCounts.get(source) === 0) {
            source.mediaElement.dispatchEvent(RELEASE_EVT);
            this.elementCounts.delete(source);
            this.elements.delete(key);
        }
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

    protected override onDisposing() {
        this.context.suspend();

        for (const userID of this.users.keys()) {
            this.removeUser(userID);
        }

        for (const clipID of this.clips.keys()) {
            this.removeClip(clipID);
        }

        super.onDisposing();
    }

    /**
     * Gets the current playback time.
     */
    get currentTime(): number {
        return this.context.currentTime;
    }

    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether the audio stream is coming from a remote user.
     */
    createSpatializer(spatialize: boolean, isRemoteStream: boolean): BaseSpatializer {
        if (!spatialize) {
            return this.noSpatializer;
        }
        else {
            const spatializer = this.destination.createSpatializer(isRemoteStream);

            spatializer.setAudioProperties(this._minDistance, this._maxDistance, this._algorithm);

            return spatializer;
        }
    }

    /**
     * Create a new user for audio processing.
     */
    createUser(userID: string, userName: string): AudioStreamSource {
        if (!this.users.has(userID)) {
            const user = new AudioStreamSource(this.context, this.createSpatializer(true, true));
            user.name = stringToName(userName, userID);
            this.users.set(userID, user);
        }

        return this.users.get(userID);
    }

    /**
     * Create a new user for the audio listener.
     */
    setLocalUserID(id: string): WebAudioDestination {
        if (this.destination) {
            this.localUserID = id;
        }

        return this.destination;
    }

    createBasicClip(id: string, asset: AssetFile, vol: number) {
        return this.createClip(id, asset, false, false, false, false, true, vol, []);
    }

    private elements = new Map<string, JuniperMediaElementAudioSourceNode>();
    private elementCounts = new Map<JuniperMediaElementAudioSourceNode, number>();

    hasClip(id: string): boolean {
        return this.clips.has(id);
    }

    /**
     * Creates a new sound effect from a series of fallback paths
     * for media files.
     * @param id - the name of the sound effect, to reference when executing playback.
     * @param asset - the element to register as a clip
     * @param looping - whether the sound effect should be played on loop.
     * @param autoPlaying - whether the sound effect should be played immediately.
     * @param spatialize - whether the sound effect should be spatialized.
     * @param randomizeStart - whether the looping sound effect should be started somewhere in the middle.
     * @param randomizePitch - whether the sound effect should be pitch-bent whenever it is played.
     * @param vol - the volume at which to set the clip.
     * @param effectNames - names of pre-canned effects to load on the control.
     * @param path - a path for loading the media of the sound effect, or the sound effect that has already been loaded.
     * @param prog - an optional callback function to use for tracking progress of loading the clip.
     */
    async createClip(
        id: string,
        asset: AssetFile | string,
        looping: boolean,
        autoPlaying: boolean,
        spatialize: boolean,
        randomizeStart: boolean,
        randomizePitch: boolean,
        vol: number,
        effectNames: string[],
        prog?: IProgress) {

        await this.ready;

        let key: string;
        let path: string;

        if (isString(asset)) {
            key = asset;
            path = await this.fetcher.get(asset)
                .progress(prog)
                .file()
                .then(unwrapResponse);
        }
        else {
            key = asset.path;
            path = asset.result;
        }

        const source = await this.getPooledSource(key, path, autoPlaying, looping);

        const clip = new AudioElementSource(
            this.context,
            source,
            randomizeStart,
            randomizePitch,
            this.createSpatializer(spatialize, false),
            ...effectNames);

        clip.addEventListener("disposing", () =>
            this.releasePooledSource(key, source));

        clip.name = stringToName("audio-clip-element", id);

        if (autoPlaying) {
            clip.play();
        }

        clip.volume = vol;
        this.clips.set(id, clip);

        return clip;
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
        if (isDefined(user.stream)) {
            user.stream = null;
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

        const clip = this.removeSource(this.clips, id);
        clip.dispose();
        return clip;
    }

    setUserStream(userID: string, stream: MediaStream): void {
        if (this.users.has(userID)) {
            const user = this.users.get(userID);
            user.stream = stream;
        }
    }

    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this._minDistance = minDistance;
        this._maxDistance = maxDistance;
        this._algorithm = algorithm;

        for (const user of this.users.values()) {
            user.setAudioProperties(this._minDistance, this._maxDistance, this.algorithm);
        }

        for (const clip of this.clips.values()) {
            clip.setAudioProperties(clip.minDistance, clip.maxDistance, this.algorithm);
        }
    }

    /**
     * Get a pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param sources - the collection of poses from which to retrieve the pose.
     * @param id - the id of the pose for which to perform the operation.
     * @param poseCallback
     */
    private withPoser<ElementT extends IAudioSource, ResultT>(sources: Map<string, ElementT>, id: string, poseCallback: withPoserCallback<ResultT>): ResultT {
        const source = sources.get(id);
        const poser = source || this.destination;
        return poseCallback(poser);
    }

    /**
     * Get a user pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the user for which to perform the operation.
     * @param poseCallback
     */
    private withUser<T>(id: string, poseCallback: withPoserCallback<T>): T {
        return this.withPoser(this.users, id, poseCallback);
    }

    /**
     * Get an audio clip pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the audio clip for which to perform the operation.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     * @param poseCallback
     */
    private withClip<T>(id: string, poseCallback: withPoserCallback<T>): T {
        return this.withPoser(this.clips, id, poseCallback);
    }

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
    setUserPose(id: string, px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void {
        this.withUser(id, (poser) => {
            poser.set(px, py, pz, qx, qy, qz, qw);
        });
    }

    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     **/
    setUserPosition(id: string, px: number, py: number, pz: number): void {
        this.withUser(id, (poser) => {
            poser.setPosition(px, py, pz);
        });
    }

    /**
     * Set the position and orientation of a user.
     * @param id - the id of the user for which to set the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setUserOrientation(id: string, qx: number, qy: number, qz: number, qw: number): void {
        this.withUser(id, (poser) => {
            poser.setOrientation(qx, qy, qz, qw);
        });
    }

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
    setClipPose(id: string, px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void {
        this.withClip(id, (poser) => {
            poser.set(px, py, pz, qx, qy, qz, qw);
        });
    }

    /**
     * Set the position and orientation of a clip.
     * @param id - the id of the user for which to set the position.
     * @param px - the horizontal component of the position.
     * @param py - the vertical component of the position.
     * @param pz - the lateral component of the position.
     **/
    setClipPosition(id: string, px: number, py: number, pz: number): void {
        this.withClip(id, (poser) => {
            poser.setPosition(px, py, pz);
        });
    }

    /**
     * Set the position and orientation of a clip.
     * @param id - the id of the user for which to set the position.
     * @param qx - the rotation quaternion x component.
     * @param qy - the rotation quaternion y component.
     * @param qz - the rotation quaternion z component.
     * @param qw - the rotation quaternion w component.
     **/
    setClipOrientation(id: string, qx: number, qy: number, qz: number, qw: number): void {
        this.withClip(id, (poser) => {
            poser.setOrientation(qx, qy, qz, qw);
        });
    }
}