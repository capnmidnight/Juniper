import { autoPlay, id, playsInline, src, srcObject } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { Audio, BackgroundAudio, elementApply, ErsatzElement, mediaElementCanPlay } from "@juniper-lib/dom/tags";
import { all } from "@juniper-lib/tslib/events/all";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { isMobileVR } from "@juniper-lib/tslib/flags";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { stringToName } from "@juniper-lib/tslib/strings/stringToName";
import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperAudioNode } from "./context/JuniperAudioNode";
import { JuniperMediaElementAudioSourceNode } from "./context/JuniperMediaElementAudioSourceNode";
import { WebAudioDestination } from "./destinations/WebAudioDestination";
import { IPoseable } from "./IPoseable";
import { WebAudioListenerNew } from "./listeners/WebAudioListenerNew";
import { WebAudioListenerOld } from "./listeners/WebAudioListenerOld";
import { LocalUserMicrophone } from "./LocalUserMicrophone";
import { AudioElementSource } from "./sources/AudioElementSource";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import type { IAudioSource } from "./sources/IAudioSource";
import { BaseSpatializer } from "./spatializers/BaseSpatializer";
import { WebAudioPannerNew } from "./spatializers/WebAudioPannerNew";
import { WebAudioPannerOld } from "./spatializers/WebAudioPannerOld";
import { SpeakerManager } from "./SpeakerManager";
import { hasNewAudioListener } from "./util";

type withPoserCallback<T> = (source: IPoseable) => T;

const USE_HEADPHONES_KEY = "juniper::useHeadphones";
const useHeadphonesToggledEvt = new TypedEvent("useheadphonestoggled");

interface AudioManagerEvents {
    useheadphonestoggled: TypedEvent<"useheadphonestoggled">;
}

/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export class AudioManager
    extends JuniperAudioNode<AudioManagerEvents>
    implements ErsatzElement {

    private readonly users = new Map<string, AudioStreamSource>();
    private readonly clips = new Map<string, AudioElementSource>();
    private readonly clipPaths = new Map<string, string>();
    private readonly pathSources = new Map<string, Promise<JuniperMediaElementAudioSourceNode>>();
    private readonly pathCounts = new Map<string, number>();

    private _minDistance = 1;
    private _maxDistance = 10;
    private _useHeadphones = false;

    private _algorithm: DistanceModelType = "inverse";
    get algorithm(): DistanceModelType { return this._algorithm; }

    readonly element: HTMLAudioElement = null;
    readonly localMic: LocalUserMicrophone;
    readonly audioDestination: WebAudioDestination = null;
    readonly speakers: SpeakerManager;
    readonly ready: Promise<void>;

    localUserID: string = null;

    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(defaultLocalUserID: string) {
        const context = new JuniperAudioContext();
        
        if ("THREE" in globalThis) {
            globalThis.THREE.AudioContext.setContext(context);
        }

        const localMic = new LocalUserMicrophone(context);

        const destination = new WebAudioDestination(
            context,
            hasNewAudioListener
                ? new WebAudioListenerNew(context)
                : new WebAudioListenerOld(context));

        super("audio-manager", context, null, null, [destination]);

        this.audioDestination = destination;
        this.localMic = localMic;

        this.element = Audio(
            id("Audio-Device-Manager"),
            display("none"),
            playsInline(true),
            autoPlay(true),
            srcObject(destination.stream));

        elementApply(document.body, this);

        this.speakers = new SpeakerManager(this.element);

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

    private async start(): Promise<void> {
        await all(
            this.context.ready.then(() => this.element.play()),
            this.speakers.ready
        );
    }

    /**
     * Gets the current playback time.
     */
    get currentTime(): number {
        return this.context.currentTime;
    }

    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether or not the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether or not the audio stream is coming from a remote user.
     */
    private createSpatializer(spatialize: boolean, isRemoteStream: boolean): BaseSpatializer {
        const destination = spatialize
            ? isRemoteStream
                ? this.audioDestination.remoteUserInput
                : this.audioDestination.spatializedInput
            : this.audioDestination.nonSpatializedInput;

        const spatializer = spatialize
            ? hasNewAudioListener
                ? new WebAudioPannerNew(this.context)
                : new WebAudioPannerOld(this.context)
            : null;

        if (isDefined(spatializer)) {
            spatializer.setAudioProperties(this._minDistance, this._maxDistance, this._algorithm);
            spatializer.connect(destination);
        }

        return spatializer;
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

    private getSourceTask(id: string, curPath: string, path: string, looping: boolean, autoPlaying: boolean, prog: IProgress): Promise<JuniperMediaElementAudioSourceNode> {
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

    private async createSourceFromFile(id: string, path: string, looping: boolean, autoPlaying: boolean, prog?: IProgress): Promise<JuniperMediaElementAudioSourceNode> {
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

    private elements = new Map<HTMLMediaElement, JuniperMediaElementAudioSourceNode>();

    private createSourceFromElement(id: string, elem: HTMLMediaElement) {
        if (!this.elements.has(elem)) {
            const node = new JuniperMediaElementAudioSourceNode(
                this.context,
                { mediaElement: elem });
            node.name = stringToName("media-element-source", id);
            this.elements.set(elem, node);
        }
        return this.elements.get(elem);
    }

    private makeClip(
        source: JuniperMediaElementAudioSourceNode,
        id: string,
        path: string,
        spatialize: boolean,
        randomize: boolean,
        autoPlaying: boolean,
        vol: number,
        ...effectNames: string[]): AudioElementSource {
        const clip = new AudioElementSource(
            this.context,
            source,
            randomize,
            this.createSpatializer(spatialize, false),
            ...effectNames);
        clip.name = stringToName("audio-clip-element", id, path);

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

        return this.removeSource(this.clips, id);
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
        const poser = source || this.audioDestination;
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
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     **/
    setUserPose(id: string, px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void {
        this.withUser(id, (poser) => {
            poser.set(px, py, pz, fx, fy, fz, ux, uy, uz);
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
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     **/
    setUserOrientation(id: string, fx: number, fy: number, fz: number): void;
    setUserOrientation(id: string, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setUserOrientation(id: string, fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        this.withUser(id, (poser) => {
            poser.setOrientation(fx, fy, fz, ux, uy, uz);
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
        this.withClip(id, (poser) => {
            poser.set(px, py, pz, fx, fy, fz, ux, uy, uz);
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
     * @param fx - the horizontal component of the forward vector.
     * @param fy - the vertical component of the forward vector.
     * @param fz - the lateral component of the forward vector.
     * @param ux - the horizontal component of the up vector.
     * @param uy - the vertical component of the up vector.
     * @param uz - the lateral component of the up vector.
     **/
    setClipOrientation(id: string, fx: number, fy: number, fz: number): void;
    setClipOrientation(id: string, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setClipOrientation(id: string, fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        this.withClip(id, (poser) => {
            poser.setOrientation(fx, fy, fz, ux, uy, uz);
        });
    }
}