import { isDefined, singleton } from "@juniper-lib/util";
import { DeviceSettingsChangedEvent, IDeviceSource, StreamChangedEvent } from "@juniper-lib/audio";
import { display, ElementChild, Muted, registerFactory, rule, SingletonStyleBlob, TypedHTMLElement, Video } from "@juniper-lib/dom";

const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";

export class LocalUserWebcamElement
    extends TypedHTMLElement<{
        devicesettingschanged: DeviceSettingsChangedEvent;
        streamchanged: StreamChangedEvent;
    }>
    implements HTMLVideoElement, IDeviceSource {

    static observedAttributes = [
        "autoplay",
        "controls",
        "controlslist",
        "crossorigin",
        "disablepictureinpicture",
        "disableremoteplayback",
        "height",
        "loop",
        "muted",
        "playsinline",
        "poster",
        "preload",
        "src",
        "width",

        "onabort",
        "onautocomplete",
        "onautocompleteerror",
        "onblur",
        "oncancel",
        "oncanplay",
        "oncanplaythrough",
        "onchange",
        "onclick",
        "onclose",
        "oncontextmenu",
        "oncuechange",
        "ondblclick",
        "ondrag",
        "ondragend",
        "ondragenter",
        "ondragleave",
        "ondragover",
        "ondragstart",
        "ondrop",
        "ondurationchange",
        "onemptied",
        "onended",
        "onerror",
        "onfocus",
        "oninput",
        "oninvalid",
        "onkeydown",
        "onkeypress",
        "onkeyup",
        "onload",
        "onloadeddata",
        "onloadedmetadata",
        "onloadstart",
        "onmousedown",
        "onmouseenter",
        "onmouseleave",
        "onmousemove",
        "onmouseout",
        "onmouseover",
        "onmouseup",
        "onmousewheel",
        "onpause",
        "onplay",
        "onplaying",
        "onprogress",
        "onratechange",
        "onreset",
        "onresize",
        "onscroll",
        "onseeked",
        "onseeking",
        "onselect",
        "onshow",
        "onsort",
        "onstalled",
        "onsubmit",
        "onsuspend",
        "ontimeupdate",
        "ontoggle",
        "onvolumechange",
        "onwaiting",

        "accesskey",
        "anchor",
        "autocapitalize",
        "autofocus",
        "class",
        "contenteditable",
        "data-*",
        "dir",
        "draggable",
        "enterkeyhint",
        "exportparts",
        "hidden",
        "id",
        "inert",
        "inputmode",
        "is",
        "itemid",
        "itemprop",
        "itemref",
        "itemscope",
        "itemtype",
        "lang",
        "nonce",
        "part",
        "popover",
        "role",
        "slot",
        "spellcheck",
        "style",
        "tabindex",
        "title",
        "translate",
        "virtualkeyboardpolicy",
        "writingsuggestions"
    ];

    attributeChangedCallback(name: string, _oldValue: string, newValue: string) {
        this.#element.setAttribute(name, newValue);
    }

    readonly #element = Video();

    #hasPermission = false;
    #device: MediaDeviceInfo = null;
    #enabled = false;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Video::LocalUserWebcamElement", () =>
            rule("local-user-webcam",
                display("contents")
            )
        );

        Object.seal(this);
    }

    get disablePictureInPicture() { return this.#element.disablePictureInPicture; }
    set disablePictureInPicture(v) { this.#element.disablePictureInPicture = v; }

    get width() { return this.#element.width; }
    set width(v) { this.#element.width = v; }

    get height() { return this.#element.height; }
    set height(v) { this.#element.height = v; }

    get videoWidth() { return this.#element.videoWidth; }
    get videoHeight() { return this.#element.videoHeight; }

    get onenterpictureinpicture() { return this.#element.onenterpictureinpicture; }
    set onenterpictureinpicture(v) { this.#element.onenterpictureinpicture = v; }

    get onleavepictureinpicture() { return this.#element.onleavepictureinpicture; }
    set onleavepictureinpicture(v) { this.#element.onleavepictureinpicture = v; }

    get playsInline() { return this.#element.playsInline; }
    set playsInline(v) { this.#element.playsInline = v; }

    get poster() { return this.#element.poster; }
    set poster(v) { this.#element.poster = v; }

    cancelVideoFrameCallback(handle: number): void {
        this.#element.cancelVideoFrameCallback(handle);
    }

    getVideoPlaybackQuality(): VideoPlaybackQuality {
        return this.#element.getVideoPlaybackQuality();
    }

    requestPictureInPicture(): Promise<PictureInPictureWindow> {
        return this.#element.requestPictureInPicture();
    }

    requestVideoFrameCallback(callback: VideoFrameRequestCallback): number {
        return this.#element.requestVideoFrameCallback(callback);
    }


    get autoplay() { return this.#element.autoplay; }
    set autoplay(v) { this.#element.autoplay = v; }

    get buffered() { return this.#element.buffered; }

    get controls() { return this.#element.controls; }
    set controls(v) { this.#element.controls = v; }

    get crossOrigin() { return this.#element.crossOrigin; }
    set crossOrigin(v) { this.#element.crossOrigin = v; }

    get currentSrc() { return this.#element.currentSrc; }

    get currentTime() { return this.#element.currentTime; }
    set currentTime(v) { this.#element.currentTime = v; }

    get defaultMuted() { return this.#element.defaultMuted; }
    set defaultMuted(v) { this.#element.defaultMuted = v; }

    get defaultPlaybackRate() { return this.#element.defaultPlaybackRate; }
    set defaultPlaybackRate(v) { this.#element.defaultPlaybackRate = v; }

    get disableRemotePlayback() { return this.#element.disableRemotePlayback; }
    set disableRemotePlayback(v) { this.#element.disableRemotePlayback = v; }

    get duration() { return this.#element.duration; }

    get ended() { return this.#element.ended; }

    get error() { return this.#element.error; }

    get loop() { return this.#element.loop; }
    set loop(v) { this.#element.loop = v; }

    get mediaKeys() { return this.#element.mediaKeys; }

    get muted() { return this.#element.muted; }
    set muted(v) { this.#element.muted = v; }

    get networkState() { return this.#element.networkState; }

    get onencrypted() { return this.#element.onencrypted; }
    set onencrypted(v) { this.#element.onencrypted = v; }

    get onwaitingforkey() { return this.#element.onwaitingforkey; }
    set onwaitingforkey(v) { this.#element.onwaitingforkey = v; }

    get paused() { return this.#element.paused; }

    get playbackRate() { return this.#element.playbackRate; }
    set playbackRate(v) { this.#element.playbackRate = v; }

    get played() { return this.#element.played; }

    get preload() { return this.#element.preload; }
    set preload(v) { this.#element.preload = v; }

    get preservesPitch() { return this.#element.preservesPitch; }
    set preservesPitch(v) { this.#element.preservesPitch = v; }

    get readyState() { return this.#element.readyState; }

    get remote() { return this.#element.remote; }

    get seekable() { return this.#element.seekable; }

    get seeking() { return this.#element.seeking; }

    get sinkId() { return this.#element.sinkId; }

    get src() { return this.#element.src; }
    set src(v) { this.#element.src = v; }

    get srcObject() { return this.#element.srcObject; }
    set srcObject(v) { this.#element.srcObject = v; }

    get textTracks() { return this.#element.textTracks; }

    get volume() { return this.#element.volume; }
    set volume(v) { this.#element.volume = v; }

    addTextTrack(kind: TextTrackKind, label?: string, language?: string): TextTrack {
        return this.#element.addTextTrack(kind, label, language);
    }

    canPlayType(type: string): CanPlayTypeResult {
        return this.#element.canPlayType(type);
    }

    fastSeek(time: number): void {
        this.#element.fastSeek(time);
    }

    load(): void {
        this.#element.load();
    }

    pause(): void {
        this.#element.pause();
    }

    play(): Promise<void> {
        return this.#element.play();
    }

    setMediaKeys(mediaKeys: MediaKeys | null): Promise<void> {
        return this.#element.setMediaKeys(mediaKeys);
    }

    setSinkId(sinkId: string): Promise<void> {
        return this.#element.setSinkId(sinkId);
    }

    get NETWORK_EMPTY() { return this.#element.NETWORK_EMPTY; }
    get NETWORK_IDLE() { return this.#element.NETWORK_IDLE; }
    get NETWORK_LOADING() { return this.#element.NETWORK_LOADING; }
    get NETWORK_NO_SOURCE() { return this.#element.NETWORK_NO_SOURCE; }
    get HAVE_NOTHING() { return this.#element.HAVE_NOTHING; }
    get HAVE_METADATA() { return this.#element.HAVE_METADATA; }
    get HAVE_CURRENT_DATA() { return this.#element.HAVE_CURRENT_DATA; }
    get HAVE_FUTURE_DATA() { return this.#element.HAVE_FUTURE_DATA; }
    get HAVE_ENOUGH_DATA() { return this.#element.HAVE_ENOUGH_DATA; }


    get mozHasAudio() { return this.#element.mozHasAudio; }
    set mozHasAudio(v) { this.#element.mozHasAudio = v; }

    get webkitAudioDecodedByteCount() { return this.#element.webkitAudioDecodedByteCount; }
    set webkitAudioDecodedByteCount(v) { this.#element.webkitAudioDecodedByteCount = v; }

    get audioTracks() { return this.#element.audioTracks; }
    set audioTracks(v) { this.#element.audioTracks = v; }

    get captureStream() { return this.#element.captureStream; }
    set captureStream(v) { this.#element.captureStream = v; }

    get mozCaptureStream() { return this.#element.mozCaptureStream; }
    set mozCaptureStream(v) { this.#element.mozCaptureStream = v; }

    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.append(this.#element);
        }
    }

    get mediaType(): "audio" | "video" {
        return "video";
    }

    get deviceKind(): MediaDeviceKind {
        return `${this.mediaType}input`;
    }

    get enabled(): boolean {
        return this.#enabled;
    }

    set enabled(v: boolean) {
        if (v !== this.enabled) {
            this.#enabled = v;
            this.onChange();
        }
    }

    get hasPermission(): boolean {
        return this.#hasPermission;
    }

    get preferredDeviceID(): string {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }

    get device() {
        return this.#device;
    }

    checkDevices(devices: MediaDeviceInfo[]): void {
        if (!this.hasPermission) {
            for (const device of devices) {
                if (device.kind === this.deviceKind
                    && device.deviceId.length > 0
                    && device.label.length > 0) {
                    this.#hasPermission = true;
                    break;
                }
            }
        }
    }

    async setDevice(device: MediaDeviceInfo) {
        if (isDefined(device) && device.kind !== this.deviceKind) {
            throw new Error(`Device is not an vide input device. Was: ${device.kind}. Label: ${device.label}`);
        }

        const curVideoID = this.device && this.device.deviceId || null;
        const nextVideoID = device && device.deviceId || null;
        if (nextVideoID !== curVideoID) {
            this.#device = device;
            localStorage.setItem(PREFERRED_VIDEO_INPUT_ID_KEY, nextVideoID);
            await this.onChange();
        }
    }

    private async onChange() {
        this.dispatchEvent(new DeviceSettingsChangedEvent());
        const oldStream = this.inStream;
        if (this.device && this.enabled) {
            this.inStream = await navigator.mediaDevices.getUserMedia({
                video: {
                    deviceId: this.device.deviceId,
                    autoGainControl: true,
                    height: 640,
                    noiseSuppression: true
                }
            });
        }
        else {
            this.inStream = null;
        }
        if (this.inStream !== oldStream) {
            this.dispatchEvent(new StreamChangedEvent(oldStream, this.outStream));
        }
    }

    get inStream(): MediaStream {
        return this.srcObject as MediaStream;
    }

    set inStream(v: MediaStream) {
        if (v !== this.inStream) {
            if (this.inStream) {
                this.#element.pause();
            }

            this.srcObject = v;

            if (this.inStream) {
                this.play();
            }
        }
    }

    get outStream() {
        return this.inStream;
    }

    stop() {
        this.inStream = null;
    }

    static install() {
        return singleton("Juniper::Audio::LocalUserWebCamElement", () => registerFactory("local-user-webcam", LocalUserWebcamElement, Muted(true)));
    }
}

export function LocalUserWebcam(...rest: ElementChild<LocalUserWebcamElement>[]) {
    return LocalUserWebcamElement.install()(...rest);
}