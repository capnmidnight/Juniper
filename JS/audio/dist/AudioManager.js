import { arrayClear, dispose, isDefined, isFunction, isObject, isString, stringToName } from "@juniper-lib/util";
import { Audio, Controls, HtmlRender, isIOS, isMobileVR, Loop, OnReleased, onUserGesture, Src } from "@juniper-lib/dom";
import { Task, TypedEvent } from "@juniper-lib/events";
import { unwrapResponse } from "@juniper-lib/fetcher";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { SpeakerManager } from "./SpeakerManager";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "./context/JuniperMediaElementAudioSourceNode";
import { WebAudioDestination } from "./destinations/WebAudioDestination";
import { AudioElementSource } from "./sources/AudioElementSource";
import { AudioStreamSource } from "./sources/AudioStreamSource";
import { NoSpatializer } from "./spatializers/NoSpatializer";
const USE_HEADPHONES_KEY = "juniper::useHeadphones";
const useHeadphonesToggledEvt = new TypedEvent("useheadphonestoggled");
const POOL_SIZE = 10;
export const RELEASE_EVT = new TypedEvent("released");
// (This is a tiny MP3 file that is silent and extremely short - retrieved from https://bigsoundbank.com and then modified)
const HAX_SRC = "data:audio/mpeg;base64,SUQzBAAAAAABEVRYWFgAAAAtAAADY29tbWVudABCaWdTb3VuZEJhbmsuY29tIC8gTGFTb25vdGhlcXVlLm9yZwBURU5DAAAAHQAAA1N3aXRjaCBQbHVzIMKpIE5DSCBTb2Z0d2FyZQBUSVQyAAAABgAAAzIyMzUAVFNTRQAAAA8AAANMYXZmNTcuODMuMTAwAAAAAAAAAAAAAAD/80DEAAAAA0gAAAAATEFNRTMuMTAwVVVVVVVVVVVVVUxBTUUzLjEwMFVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVf/zQsRbAAADSAAAAABVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVf/zQMSkAAADSAAAAABVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV";
/**
 * A manager of audio sources, destinations, and their spatialization.
 **/
export class AudioManager extends BaseNodeCluster {
    get algorithm() { return this._algorithm; }
    get ready() {
        return this._ready;
    }
    get isReady() {
        return this._ready.finished
            && this._ready.resolved;
    }
    /**
     * Creates a new manager of audio sources, destinations, and their spatialization.
     **/
    constructor(fetcher, defaultLocalUserID) {
        const context = new JuniperAudioContext();
        const x = globalThis;
        if ("THREE" in x
            && isObject(x.THREE)
            && "AudioContext" in x.THREE
            && isObject(x.THREE.AudioContext)
            && "setContext" in x.THREE.AudioContext
            && isFunction(x.THREE.AudioContext.setContext)) {
            x.THREE.AudioContext.setContext(context);
        }
        const destination = new WebAudioDestination(context);
        const noSpatializer = new NoSpatializer(destination.nonSpatializedInput);
        const speakers = new SpeakerManager(destination.audioElement);
        super("audio-manager", context, null, null, [noSpatializer, destination]);
        this.fetcher = fetcher;
        this.users = new Map();
        this.clips = new Map();
        this.pendingAudio = new Array();
        this.audioPool = new Array();
        this.elements = new Map();
        this.elementCounts = new Map();
        this._minDistance = 0.25;
        this._maxDistance = 100;
        this._algorithm = "inverse";
        this._useHeadphones = false;
        this.destination = null;
        this._ready = new Task();
        this.localUserID = null;
        this.destination = destination;
        this.noSpatializer = noSpatializer;
        this.speakers = speakers;
        Promise.all([
            this.context.ready,
            this.destination.ready,
            this.speakers.ready
        ]).then(() => this._ready.resolve());
        this.setLocalUserID(defaultLocalUserID);
        const useHeadphones = localStorage.getItem(USE_HEADPHONES_KEY);
        if (isDefined(useHeadphones)) {
            this._useHeadphones = useHeadphones === "true";
        }
        else {
            this._useHeadphones = isMobileVR();
        }
        this.enpool();
        if (isIOS()) {
            onUserGesture(() => {
                const depooling = [...this.pendingAudio];
                arrayClear(this.pendingAudio);
                for (const p of depooling) {
                    p();
                }
            }, true);
        }
        Object.seal(this);
    }
    enpool() {
        for (let i = 0; i < POOL_SIZE; ++i) {
            const task = new Task();
            const audio = Audio(Src(HAX_SRC), Controls(false), OnReleased(() => {
                audio.pause();
                audio.src = HAX_SRC;
            }));
            this.audioPool.push(task);
            if (isIOS()) {
                this.pendingAudio.push(() => {
                    audio.play();
                    task.resolve(audio);
                });
            }
            else {
                task.resolve(audio);
            }
        }
    }
    preparePool(size) {
        while (this.audioPool.length < size) {
            this.enpool();
        }
    }
    async getPooledAudio(...rest) {
        if (this.audioPool.length === 0) {
            throw new Error("Audio pool exhausted!");
        }
        const audioTask = this.audioPool.shift();
        if (this.audioPool.length <= POOL_SIZE / 2) {
            this.enpool();
        }
        const audio = await audioTask;
        audio.pause();
        HtmlRender(audio, ...rest);
        return audio;
    }
    async getPooledSource(key, path, looping) {
        if (!this.elements.has(key)) {
            const mediaElement = await this.getPooledAudio(Src(path), Loop(looping && !isIOS()));
            if (isIOS() && looping) {
                mediaElement.addEventListener("ended", () => mediaElement.play());
            }
            const node = new JuniperMediaElementAudioSourceNode(this.context, { mediaElement });
            node.name = stringToName("media-element-source", key);
            this.elements.set(key, node);
            this.elementCounts.set(key, 0);
        }
        const source = this.elements.get(key);
        this.elementCounts.set(key, this.elementCounts.get(key) + 1);
        return source;
    }
    releasePooledSource(key) {
        const source = this.elements.get(key);
        this.elementCounts.set(key, this.elementCounts.get(key) - 1);
        if (this.elementCounts.get(key) === 0) {
            source.mediaElement.dispatchEvent(RELEASE_EVT);
            this.elementCounts.delete(key);
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
    onDisposing() {
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
    get currentTime() {
        return this.context.currentTime;
    }
    /**
     * Creates a spatialzer for an audio source.
     * @param spatialize - whether the audio stream should be spatialized. Stereo audio streams that are spatialized will get down-mixed to a single channel.
     * @param isRemoteStream - whether the audio stream is coming from a remote user.
     */
    createSpatializer(spatialize, isRemoteStream) {
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
    createUser(userID, userName) {
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
    setLocalUserID(id) {
        if (this.destination) {
            this.localUserID = id;
        }
        return this.destination;
    }
    createBasicClip(id, asset, vol) {
        return this.createClip(id, asset, false, false, false, true, vol, []);
    }
    hasClip(id) {
        return this.clips.has(id);
    }
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
    async createClip(id, asset, looping, spatialize, randomizeStart, randomizePitch, vol, effectNames, prog) {
        await this.ready;
        let key;
        let path;
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
        const source = await this.getPooledSource(key, path, looping);
        const clip = new AudioElementSource(this.context, source, randomizeStart, randomizePitch, this.createSpatializer(spatialize, false), ...effectNames);
        clip.addEventListener("disposing", () => this.releasePooledSource(key));
        clip.name = stringToName("audio-clip-element", id);
        clip.volume = vol;
        this.clips.set(id, clip);
        return clip;
    }
    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has started playing.
     * @param id - the name of the effect to play.
     */
    async playClip(id) {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            await clip.play();
        }
    }
    /**
     * Plays a named sound effect, with the returned promise resolving when the clip has finished playing.
     * @param id - the name of the effect to play.
     */
    async playClipThrough(id) {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            await clip.playThrough();
        }
    }
    stopClip(id) {
        if (this.hasClip(id)) {
            const clip = this.clips.get(id);
            clip.stop();
        }
    }
    /**
     * Get an existing user.
     */
    getUser(userID) {
        return this.users.get(userID);
    }
    /**
     * Get an existing audio clip.
     */
    getClip(id) {
        return this.clips.get(id);
    }
    /**
     * Remove an audio source from audio processing.
     * @param sources - the collection of audio sources from which to remove.
     * @param id - the id of the audio source to remove
     **/
    removeSource(sources, id) {
        const source = sources.get(id);
        if (isDefined(source)) {
            sources.delete(id);
            dispose(source);
        }
        return source;
    }
    /**
     * Remove a user from audio processing.
     **/
    removeUser(userID) {
        const user = this.removeSource(this.users, userID);
        if (isDefined(user.stream)) {
            user.stream = null;
        }
    }
    /**
     * Remove an audio clip from audio processing.
     **/
    removeClip(id) {
        const clip = this.removeSource(this.clips, id);
        dispose(clip);
        return clip;
    }
    setUserStream(userID, stream) {
        if (this.users.has(userID)) {
            const user = this.users.get(userID);
            user.stream = stream;
        }
    }
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance, maxDistance, algorithm) {
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
    withPoser(sources, id, poseCallback) {
        const source = sources.get(id);
        const poser = source || this.destination;
        return poseCallback(poser);
    }
    /**
     * Get a user pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the user for which to perform the operation.
     * @param poseCallback
     */
    withUser(id, poseCallback) {
        return this.withPoser(this.users, id, poseCallback);
    }
    /**
     * Get an audio clip pose, normalize the transition time, and perform on operation on it, if it exists.
     * @param id - the id of the audio clip for which to perform the operation.
     * @param dt - the amount of time to take to make the transition. Defaults to this AudioManager's `transitionTime`.
     * @param poseCallback
     */
    withClip(id, poseCallback) {
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
    setUserPose(id, px, py, pz, qx, qy, qz, qw) {
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
    setUserPosition(id, px, py, pz) {
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
    setUserOrientation(id, qx, qy, qz, qw) {
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
    setClipPose(id, px, py, pz, qx, qy, qz, qw) {
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
    setClipPosition(id, px, py, pz) {
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
    setClipOrientation(id, qx, qy, qz, qw) {
        this.withClip(id, (poser) => {
            poser.setOrientation(qx, qy, qz, qw);
        });
    }
}
//# sourceMappingURL=AudioManager.js.map