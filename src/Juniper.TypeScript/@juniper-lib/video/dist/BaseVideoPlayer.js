import { RELEASE_EVT } from "@juniper-lib/audio/dist/AudioManager";
import { JuniperMediaElementAudioSourceNode } from "@juniper-lib/audio/dist/context/JuniperMediaElementAudioSourceNode";
import { audioRecordSorter } from "@juniper-lib/audio/dist/data";
import { BaseAudioSource } from "@juniper-lib/audio/dist/sources/BaseAudioSource";
import { MediaElementSourceLoadedEvent, MediaElementSourcePausedEvent, MediaElementSourcePlayedEvent, MediaElementSourceProgressEvent, MediaElementSourceStoppedEvent } from "@juniper-lib/audio/dist/sources/IPlayable";
import { MediaPlayerLoadingEvent } from "@juniper-lib/audio/dist/sources/IPlayer";
import { PriorityList } from "@juniper-lib/collections/dist/PriorityList";
import { AutoPlay, Controls, Loop } from "@juniper-lib/dom/dist/attrs";
import { Audio, Video, mediaElementCanPlayThrough } from "@juniper-lib/dom/dist/tags";
import { once } from "@juniper-lib/events/dist/once";
import { Video_Vendor_Mpeg_Dash_Mpd } from "@juniper-lib/mediatypes";
import { progressTasks } from "@juniper-lib/progress/dist/progressTasks";
import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { isVideoRecord } from "./data";
export class BaseVideoPlayer extends BaseAudioSource {
    get data() {
        return this._data;
    }
    get loaded() {
        return this._loaded;
    }
    get title() {
        return this.video.title;
    }
    setTitle(v) {
        this.video.title = v;
        this.audio.title = v;
    }
    constructor(type, context, spatializer) {
        const video = BaseVideoPlayer.createMediaElement(Video, Controls(true));
        const audio = BaseVideoPlayer.createMediaElement(Audio, Controls(false));
        const videoNode = new JuniperMediaElementAudioSourceNode(context, {
            mediaElement: video
        });
        videoNode.name = `${type}-video`;
        const audioNode = new JuniperMediaElementAudioSourceNode(context, {
            mediaElement: audio
        });
        audioNode.name = `${type}-audio`;
        super(type, context, spatializer, [], [videoNode, audioNode]);
        this.onTimeUpdate = null;
        this.wasUsingAudioElement = false;
        this.nextStartTime = null;
        this._data = null;
        this._loaded = false;
        this.onError = new Map();
        this.sourcesByURL = new Map();
        this.sources = new PriorityList();
        this.potatoes = new PriorityList();
        videoNode.connect(this.volumeControl);
        audioNode.connect(this.volumeControl);
        this.video = video;
        this.audio = audio;
        this.loadingEvt = new MediaPlayerLoadingEvent(this);
        this.loadEvt = new MediaElementSourceLoadedEvent(this);
        this.playEvt = new MediaElementSourcePlayedEvent(this);
        this.pauseEvt = new MediaElementSourcePausedEvent(this);
        this.stopEvt = new MediaElementSourceStoppedEvent(this);
        this.progEvt = new MediaElementSourceProgressEvent(this);
        this.onSeeked = () => {
            if (this.useAudioElement) {
                this.audio.currentTime = this.video.currentTime;
            }
        };
        this.onPlay = async () => {
            this.onSeeked();
            if (this.useAudioElement) {
                await this.context.ready;
                await this.audio.play();
            }
            this.dispatchEvent(this.playEvt);
        };
        this.onPause = (evt) => {
            if (this.useAudioElement) {
                this.onSeeked();
                this.audio.pause();
            }
            if (this.video.currentTime === 0 || evt.type === "ended") {
                this.dispatchEvent(this.stopEvt);
            }
            else {
                this.dispatchEvent(this.pauseEvt);
            }
        };
        let wasWaiting = false;
        this.onWaiting = () => {
            if (this.useAudioElement) {
                wasWaiting = true;
                this.audio.pause();
            }
        };
        this.onCanPlay = async () => {
            if (this.useAudioElement && wasWaiting) {
                await this.context.ready;
                await this.audio.play();
                wasWaiting = false;
            }
        };
        this.wasUsingAudioElement = false;
        this.onTimeUpdate = async () => {
            const quality = this.video.getVideoPlaybackQuality();
            if (quality.totalVideoFrames === 0) {
                const onError = this.onError.get(this.video);
                if (isDefined(onError)) {
                    await onError();
                }
            }
            else if (this.useAudioElement) {
                this.wasUsingAudioElement = false;
                const delta = this.video.currentTime - this.audio.currentTime;
                if (Math.abs(delta) > 0.25) {
                    this.audio.currentTime = this.video.currentTime;
                }
            }
            else if (!this.wasUsingAudioElement) {
                this.wasUsingAudioElement = true;
                this.audio.pause();
            }
            this.progEvt.value = this.video.currentTime;
            this.progEvt.total = this.video.duration;
            this.dispatchEvent(this.progEvt);
        };
        this.video.addEventListener("seeked", this.onSeeked);
        this.video.addEventListener("play", this.onPlay);
        this.video.addEventListener("pause", this.onPause);
        this.video.addEventListener("ended", this.onPause);
        this.video.addEventListener("waiting", this.onWaiting);
        this.video.addEventListener("canplay", this.onCanPlay);
        this.video.addEventListener("timeupdate", this.onTimeUpdate);
        Object.assign(window, { videoPlayer: this });
    }
    elementHasAudio(elem) {
        const source = this.sourcesByURL.get(elem.src);
        return isDefined(source) && source.acodec !== "none"
            || isDefined(elem.audioTracks) && elem.audioTracks.length > 0
            || isDefined(elem.webkitAudioDecodedByteCount) && elem.webkitAudioDecodedByteCount > 0
            || isDefined(elem.mozHasAudio) && elem.mozHasAudio;
    }
    get useAudioElement() {
        return !this.elementHasAudio(this.video)
            && this.elementHasAudio(this.audio);
    }
    onDisposing() {
        this.clear();
        this.video.removeEventListener("seeked", this.onSeeked);
        this.video.removeEventListener("play", this.onPlay);
        this.video.removeEventListener("pause", this.onPause);
        this.video.removeEventListener("ended", this.onPause);
        this.video.removeEventListener("waiting", this.onWaiting);
        this.video.removeEventListener("canplay", this.onCanPlay);
        this.video.removeEventListener("timeupdate", this.onTimeUpdate);
        super.onDisposing();
        this.audio.dispatchEvent(RELEASE_EVT);
        this.video.dispatchEvent(RELEASE_EVT);
    }
    clear() {
        this.stop();
        for (const [elem, onError] of this.onError) {
            elem.removeEventListener("error", onError);
        }
        this.onError.clear();
        this.sourcesByURL.clear();
        this.sources.clear();
        this.potatoes.clear();
        this.video.src = "";
        this.audio.src = "";
        this.wasUsingAudioElement = false;
        this._data = null;
        this._loaded = false;
    }
    async load(data, prog) {
        this.clear();
        this._data = data;
        if (isString(data)) {
            this.setTitle(data);
            this.potatoes.add(this.video, data);
        }
        else {
            this.setTitle(data.title);
            this.fillSources(this.video, data.videos);
            this.fillSources(this.audio, data.audios);
        }
        if (!this.hasSources(this.video)) {
            throw new Error("No video sources found");
        }
        this.dispatchEvent(this.loadingEvt);
        await progressTasks(prog, (prog) => this.loadMediaElement(this.audio, prog), (prog) => this.loadMediaElement(this.video, prog));
        if (isString(data)) {
            this.nextStartTime = null;
        }
        else {
            this.nextStartTime = data.startTime;
        }
        if (!this.hasSources(this.video)) {
            throw new Error("No video playable sources");
        }
        this._loaded = true;
        this.dispatchEvent(this.loadEvt);
        return this;
    }
    fillSources(elem, formats) {
        formats.sort(audioRecordSorter);
        for (const format of formats) {
            if (!Video_Vendor_Mpeg_Dash_Mpd.matches(format.contentType)) {
                this.sources.add(elem, format);
                this.sourcesByURL.set(format.url, format);
            }
        }
    }
    static createMediaElement(MediaElement, ...rest) {
        return MediaElement(AutoPlay(false), Loop(false), ...rest);
    }
    async getMediaCapabilities(source) {
        const config = {
            type: "file"
        };
        if (isVideoRecord(source)) {
            config.video = {
                contentType: source.contentType,
                bitrate: source.vbr * 1024,
                framerate: source.fps,
                width: source.width,
                height: source.height
            };
        }
        else if (source.acodec !== "none") {
            config.audio = {
                contentType: source.contentType,
                bitrate: source.abr * 1024,
                samplerate: source.asr
            };
        }
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
    hasSources(elem) {
        return this.sources.get(elem).length > 0
            || this.potatoes.count(elem) > 0;
    }
    async loadMediaElement(elem, prog) {
        if (isDefined(prog)) {
            prog.start();
        }
        if (this.onError.has(elem)) {
            elem.removeEventListener("error", this.onError.get(elem));
            this.onError.delete(elem);
        }
        while (this.hasSources(elem)) {
            let url = null;
            const source = this.sources.get(elem).shift();
            if (isDefined(source)) {
                const caps = await this.getMediaCapabilities(source);
                if (!caps.smooth || !caps.powerEfficient) {
                    this.potatoes.add(elem, source.url);
                    continue;
                }
                else {
                    url = source.url;
                }
            }
            else {
                url = this.potatoes.get(elem).shift();
            }
            elem.src = url;
            elem.load();
            if (await mediaElementCanPlayThrough(elem)) {
                if (isDefined(source)) {
                    this.sources.get(elem).unshift(source);
                }
                else {
                    this.potatoes.get(elem).unshift(url);
                }
                const onError = () => this.loadMediaElement(elem, prog);
                elem.addEventListener("error", onError);
                this.onError.set(elem, onError);
                this.wasUsingAudioElement = this.wasUsingAudioElement;
                if (isDefined(prog)) {
                    prog.end();
                }
                return;
            }
        }
    }
    get width() {
        return this.video.videoWidth;
    }
    get height() {
        return this.video.videoHeight;
    }
    get playbackState() {
        if (isNullOrUndefined(this.data)) {
            return "empty";
        }
        if (!this.loaded) {
            return "loading";
        }
        if (this.video.error) {
            return "errored";
        }
        if (this.video.ended
            || this.video.paused
                && this.video.currentTime === 0) {
            return "stopped";
        }
        if (this.video.paused) {
            return "paused";
        }
        return "playing";
    }
    async play() {
        await this.context.ready;
        if (isDefined(this.nextStartTime) && this.nextStartTime > 0) {
            this.video.pause();
            this.video.currentTime = this.nextStartTime;
            this.nextStartTime = null;
        }
        await this.video.play();
    }
    async playThrough() {
        const endTask = once(this, "stopped");
        await this.play();
        await endTask;
    }
    pause() {
        this.video.pause();
    }
    stop() {
        this.pause();
        this.video.currentTime = 0;
    }
    restart() {
        this.stop();
        return this.play();
    }
}
//# sourceMappingURL=BaseVideoPlayer.js.map