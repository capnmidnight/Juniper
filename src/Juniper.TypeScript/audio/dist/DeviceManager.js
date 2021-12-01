import { arrayScan, arraySortByKey, isDefined, isFunction, isNullOrUndefined, TypedEvent, TypedEventBase } from "juniper-tslib";
/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export const canChangeAudioOutput = isFunction(HTMLAudioElement.prototype.setSinkId);
function filterDeviceDuplicates(devices) {
    const filtered = [];
    for (let i = 0; i < devices.length; ++i) {
        const a = devices[i];
        let found = false;
        for (let j = 0; j < filtered.length && !found; ++j) {
            const b = filtered[j];
            found = a.kind === b.kind && b.label.indexOf(a.label) > 0;
        }
        if (!found) {
            filtered.push(a);
        }
    }
    return filtered;
}
export class DeviceManagerAudioOutputChangedEvent extends TypedEvent {
    device;
    constructor(device) {
        super("audiooutputchanged");
        this.device = device;
    }
}
export class DeviceManagerAudioInputChangedEvent extends TypedEvent {
    audio;
    constructor(audio) {
        super("audioinputchanged");
        this.audio = audio;
    }
}
export class DeviceManagerVideoInputChangedEvent extends TypedEvent {
    video;
    constructor(video) {
        super("videoinputchanged");
        this.video = video;
    }
}
const PREFERRED_AUDIO_OUTPUT_ID_KEY = "calla:preferredAudioOutputID";
const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";
const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";
export class DeviceManager extends TypedEventBase {
    element;
    needsVideoDevice;
    _hasAudioPermission = false;
    get hasAudioPermission() {
        return this._hasAudioPermission;
    }
    _hasVideoPermission = false;
    get hasVideoPermission() {
        return this._hasVideoPermission;
    }
    _currentStream = null;
    get currentStream() {
        return this._currentStream;
    }
    set currentStream(v) {
        if (v !== this.currentStream) {
            if (isDefined(this.currentStream)
                && this.currentStream.active) {
                for (const track of this.currentStream.getTracks()) {
                    track.stop();
                }
            }
            this._currentStream = v;
        }
    }
    ready;
    constructor(element, needsVideoDevice = false) {
        super();
        this.element = element;
        this.needsVideoDevice = needsVideoDevice;
        this.ready = this.start();
        Object.seal(this);
    }
    async start() {
        if (canChangeAudioOutput) {
            const device = await this.getPreferredAudioOutput();
            if (device) {
                await this.setAudioOutputDevice(device);
            }
        }
    }
    async startPreferredAudioInput() {
        const device = await this.getPreferredAudioInput();
        if (device) {
            await this.setAudioInputDevice(device);
            this.currentStream = await this.startStream();
        }
    }
    async startPreferredVideoInput() {
        const device = await this.getPreferredVideoInput();
        if (device) {
            await this.setVideoInputDevice(device);
            this.currentStream = await this.startStream();
        }
    }
    get preferredAudioOutputID() {
        if (!canChangeAudioOutput) {
            return null;
        }
        return localStorage.getItem(PREFERRED_AUDIO_OUTPUT_ID_KEY);
    }
    get preferredAudioInputID() {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }
    get preferredVideoInputID() {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }
    async getAudioOutputDevices(filterDuplicates = false) {
        if (!canChangeAudioOutput) {
            return [];
        }
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.audioOutput || [];
    }
    async getAudioInputDevices(filterDuplicates = false) {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.audioInput || [];
    }
    async getVideoInputDevices(filterDuplicates = false) {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.videoInput || [];
    }
    async getAudioOutputDevice() {
        if (!canChangeAudioOutput) {
            return null;
        }
        const curId = this.element && this.element.sinkId;
        if (isNullOrUndefined(curId)) {
            return null;
        }
        const devices = await this.getAudioOutputDevices(), device = arrayScan(devices, d => d.deviceId === curId);
        return device;
    }
    async getAudioInputDevice() {
        if (isNullOrUndefined(this.currentStream)
            || !this.currentStream.active) {
            return null;
        }
        const curTracks = this.currentStream.getAudioTracks();
        if (curTracks.length === 0) {
            return null;
        }
        const testTrack = curTracks[0];
        const devices = await this.getAudioInputDevices();
        const device = arrayScan(devices, d => testTrack.label === d.label);
        return device;
    }
    async getVideoInputDevice() {
        if (isNullOrUndefined(this.currentStream)
            || !this.currentStream.active) {
            return null;
        }
        const curTracks = this.currentStream.getVideoTracks();
        if (curTracks.length === 0) {
            return null;
        }
        const testTrack = curTracks[0];
        const devices = await this.getVideoInputDevices();
        const device = arrayScan(devices, d => testTrack.label === d.label);
        return device;
    }
    async getPreferredAudioOutput() {
        if (!canChangeAudioOutput) {
            return null;
        }
        const devices = await this.getAudioOutputDevices();
        const device = arrayScan(devices, (d) => d.deviceId === this.preferredAudioOutputID, (d) => d.deviceId === "default", (d) => d.deviceId.length > 0);
        return device;
    }
    async getPreferredAudioInput() {
        const devices = await this.getAudioInputDevices();
        const device = arrayScan(devices, (d) => d.deviceId === this.preferredAudioInputID, (d) => d.deviceId === "default", (d) => d.deviceId.length > 0);
        return device;
    }
    async getPreferredVideoInput() {
        const devices = await this.getVideoInputDevices();
        const device = arrayScan(devices, (d) => d.deviceId === this.preferredVideoInputID, (d) => this.needsVideoDevice && d.deviceId.length > 0);
        return device;
    }
    async setAudioOutputDevice(device) {
        if (canChangeAudioOutput) {
            if (isDefined(device) && device.kind !== "audiooutput") {
                throw new Error(`Device is not an audio output device. Was: ${device.kind}. Label: ${device.label}`);
            }
            localStorage.setItem(PREFERRED_AUDIO_OUTPUT_ID_KEY, device && device.deviceId || null);
            const curDevice = this.element;
            const curDeviceID = curDevice && curDevice.sinkId;
            if (this.preferredAudioOutputID !== curDeviceID) {
                if (isDefined(this.preferredAudioOutputID)) {
                    await this.element.setSinkId(this.preferredAudioOutputID);
                }
                this.dispatchEvent(new DeviceManagerAudioOutputChangedEvent(device));
            }
        }
    }
    async setAudioInputDevice(device) {
        if (isDefined(device) && device.kind !== "audioinput") {
            throw new Error(`Device is not an audio input device. Was: ${device.kind}. Label: ${device.label}`);
        }
        localStorage.setItem(PREFERRED_AUDIO_INPUT_ID_KEY, device && device.deviceId || null);
        const curAudio = await this.getAudioInputDevice();
        const curAudioID = curAudio && curAudio.deviceId;
        if (this.preferredAudioInputID !== curAudioID) {
            this.dispatchEvent(new DeviceManagerAudioInputChangedEvent(device));
        }
    }
    async setVideoInputDevice(device) {
        if (isDefined(device) && device.kind !== "videoinput") {
            throw new Error(`Device is not an video input device. Was: ${device.kind}. Label: ${device.label}`);
        }
        localStorage.setItem(PREFERRED_VIDEO_INPUT_ID_KEY, device && device.deviceId || null);
        const curVideo = await this.getVideoInputDevice();
        const curVideoID = curVideo && curVideo.deviceId;
        if (this.preferredVideoInputID !== curVideoID) {
            this.dispatchEvent(new DeviceManagerVideoInputChangedEvent(device));
        }
    }
    async getAvailableDevices(filterDuplicates = false) {
        let devices = await this.getDevices();
        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }
        return {
            audioOutput: canChangeAudioOutput ? devices.filter(d => d.kind === "audiooutput") : [],
            audioInput: devices.filter(d => d.kind === "audioinput"),
            videoInput: devices.filter(d => d.kind === "videoinput")
        };
    }
    async getDevices() {
        let devices = null;
        let testStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();
            for (const device of devices) {
                if (device.deviceId.length > 0) {
                    if (!this.hasAudioPermission) {
                        this._hasAudioPermission = device.kind === "audioinput"
                            && device.label.length > 0;
                    }
                    if (this.needsVideoDevice && !this.hasVideoPermission) {
                        this._hasVideoPermission = device.kind === "videoinput"
                            && device.label.length > 0;
                    }
                }
            }
            if (this.hasAudioPermission
                && (!this.needsVideoDevice || this.hasVideoPermission)) {
                break;
            }
            try {
                if (!this.hasAudioPermission
                    || this.needsVideoDevice && !this.hasVideoPermission) {
                    testStream = await this.startStream();
                }
            }
            catch (exp) {
                console.warn(exp);
            }
        }
        if (testStream) {
            for (const track of testStream.getTracks()) {
                track.stop();
            }
        }
        devices = arraySortByKey(devices || [], d => d.label);
        return devices;
    }
    startStream() {
        return navigator.mediaDevices.getUserMedia({
            audio: this.preferredAudioInputID
                && { deviceId: this.preferredAudioInputID }
                || true,
            video: this.needsVideoDevice
                && (this.preferredVideoInputID
                    && { deviceId: this.preferredVideoInputID }
                    || true)
                || false
        });
    }
    async getMediaPermissions() {
        await this.getDevices();
        return {
            audio: this.hasAudioPermission,
            video: this.hasVideoPermission
        };
    }
}
