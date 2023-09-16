import { arrayClear, arrayReplace } from "@juniper-lib/collections/dist/arrays";
import { AutoPlay, Controls, Loop } from "@juniper-lib/dom/dist/attrs";
import { Audio, mediaElementCanPlayThrough } from "@juniper-lib/dom/dist/tags";
import { once } from "@juniper-lib/events/dist/once";
import { URLBuilder } from "@juniper-lib/tslib/dist/URLBuilder";
import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { RELEASE_EVT } from "../AudioManager";
import { JuniperMediaElementAudioSourceNode } from "../context/JuniperMediaElementAudioSourceNode";
import { audioRecordSorter } from "../data";
import { BaseAudioSource } from "./BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "./IPlayable";
import { MediaPlayerLoadingEvent } from "./IPlayer";
export class AudioPlayer extends BaseAudioSource {
    get data() {
        return this._data;
    }
    get loaded() {
        return this._loaded;
    }
    get title() {
        return this.audioElement.title;
    }
    setTitle(v) {
        this.audioElement.title = v;
    }
    constructor(context, spatializer) {
        const mediaElement = Audio(AutoPlay(false), Loop(false), Controls(true));
        const elementNode = new JuniperMediaElementAudioSourceNode(context, {
            mediaElement
        });
        elementNode.name = "JuniperAudioPlayer-Input";
        super("audio-player", context, spatializer, [], [elementNode]);
        this.cacheBustSources = new Map();
        this._data = null;
        this._loaded = false;
        this.sourcesByURL = new Map();
        this.sources = new Array();
        this.potatoes = new Array();
        elementNode.connect(this.volumeControl);
        this.audioElement = mediaElement;
        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);
        this.onPlay = async () => {
            this.enable();
            this.dispatchEvent(this.playEvt);
        };
        this.onPause = (evt) => {
            this.disable();
            if (this.audioElement.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };
        this.onTimeUpdate = async () => {
            this.progEvt.value = this.audioElement.currentTime;
            this.progEvt.total = this.audioElement.duration;
            this.dispatchEvent(this.progEvt);
        };
        this.onError = () => this.loadAudio();
        this.audioElement.addEventListener("play", this.onPlay);
        this.audioElement.addEventListener("pause", this.onPause);
        this.audioElement.addEventListener("ended", this.onPause);
        this.audioElement.addEventListener("error", this.onError);
        this.audioElement.addEventListener("waiting", this.onWaiting);
        this.audioElement.addEventListener("canplay", this.onCanPlay);
        this.audioElement.addEventListener("timeupdate", this.onTimeUpdate);
        Object.assign(window, { audioPlayer: this });
    }
    get hasAudio() {
        const source = this.sourcesByURL.get(this.audioElement.src);
        return isDefined(source) && source.acodec !== "none"
            || isDefined(this.audioElement.audioTracks) && this.audioElement.audioTracks.length > 0
            || isDefined(this.audioElement.webkitAudioDecodedByteCount) && this.audioElement.webkitAudioDecodedByteCount > 0
            || isDefined(this.audioElement.mozHasAudio) && this.audioElement.mozHasAudio;
    }
    onDisposing() {
        super.onDisposing();
        this.clear();
        this.audioElement.removeEventListener("play", this.onPlay);
        this.audioElement.removeEventListener("pause", this.onPause);
        this.audioElement.removeEventListener("ended", this.onPause);
        this.audioElement.removeEventListener("error", this.onError);
        this.audioElement.removeEventListener("waiting", this.onWaiting);
        this.audioElement.removeEventListener("canplay", this.onCanPlay);
        this.audioElement.removeEventListener("timeupdate", this.onTimeUpdate);
        this.audioElement.dispatchEvent(RELEASE_EVT);
    }
    clear() {
        this.stop();
        this.sourcesByURL.clear();
        arrayClear(this.sources);
        arrayClear(this.potatoes);
        this.audioElement.src = "";
        this._data = null;
        this._loaded = false;
    }
    cacheBust(data) {
        const curCount = this.cacheBustSources.get(data) || 0;
        this.cacheBustSources.set(data, curCount + 1);
    }
    async load(data, prog) {
        this.clear();
        this._data = data;
        if (isString(data)) {
            this.setTitle(data);
            this.potatoes.push(data);
        }
        else {
            this.setTitle(data.title);
            data.audios.sort(audioRecordSorter);
            arrayReplace(this.sources, ...data.audios);
        }
        for (const audio of this.sources) {
            this.sourcesByURL.set(audio.url, audio);
        }
        if (!this.hasSources) {
            throw new Error("No audio sources");
        }
        this.dispatchEvent(this.loadingEvt);
        await this.loadAudio(prog);
        if (!this.hasSources) {
            throw new Error("No audio sources");
        }
        this._loaded = true;
        this.dispatchEvent(this.loadEvt);
        return this;
    }
    async getMediaCapabilities(source) {
        const config = {
            type: "file",
            audio: {
                contentType: source.contentType,
                bitrate: source.abr * 1024,
                samplerate: source.asr
            }
        };
        try {
            return await navigator.mediaCapabilities.decodingInfo(config);
        }
        catch {
            return {
                supported: true,
                powerEfficient: false,
                smooth: false,
                configuration: config
            };
        }
    }
    get hasSources() {
        return this.sources.length > 0
            || this.potatoes.length > 0;
    }
    async loadAudio(prog) {
        if (isDefined(prog)) {
            prog.start();
        }
        this.audioElement.removeEventListener("error", this.onError);
        while (this.hasSources) {
            let url = null;
            const source = this.sources.shift();
            if (isDefined(source)) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potatoes.push(source.url);
                    continue;
                }
                else {
                    url = source.url;
                }
            }
            else {
                url = this.potatoes.shift();
            }
            const cacheV = this.cacheBustSources.get(this.data);
            if (isDefined(cacheV)) {
                const uri = new URLBuilder(url, location.href);
                uri.query("v", cacheV.toString());
                url = uri.toString();
            }
            this.audioElement.src = url;
            this.audioElement.load();
            if (await mediaElementCanPlayThrough(this.audioElement)) {
                if (isDefined(source)) {
                    this.sources.unshift(source);
                }
                else {
                    this.potatoes.unshift(url);
                }
                this.audioElement.addEventListener("error", this.onError);
                if (isDefined(prog)) {
                    prog.end();
                }
                return;
            }
        }
    }
    get playbackState() {
        if (isNullOrUndefined(this.data)) {
            return "empty";
        }
        if (!this.loaded) {
            return "loading";
        }
        if (this.audioElement.error) {
            return "errored";
        }
        if (this.audioElement.ended
            || this.audioElement.paused
                && this.audioElement.currentTime === 0) {
            return "stopped";
        }
        if (this.audioElement.paused) {
            return "paused";
        }
        return "playing";
    }
    async play() {
        await this.context.ready;
        await this.audioElement.play();
    }
    async playThrough() {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }
    pause() {
        this.audioElement.pause();
    }
    stop() {
        this.pause();
        this.audioElement.currentTime = 0;
    }
    restart() {
        this.stop();
        return this.play();
    }
}
//# sourceMappingURL=AudioPlayer.js.map