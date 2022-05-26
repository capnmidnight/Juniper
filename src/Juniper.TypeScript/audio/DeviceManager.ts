import { arrayScan, arraySortByKey, isDefined, isFunction, isNullOrUndefined, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import type { MediaDeviceSet } from "./MediaDeviceSet";
import type { MediaPermissionSet } from "./MediaPermissionSet";

/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export const canChangeAudioOutput = isFunction((HTMLAudioElement.prototype as any).setSinkId);


function filterDeviceDuplicates(devices: MediaDeviceInfo[]) {
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

export class DeviceManagerAudioOutputChangedEvent
    extends TypedEvent<"audiooutputchanged"> {
    public constructor(public readonly device: MediaDeviceInfo) {
        super("audiooutputchanged");
    }
}

export class DeviceManagerAudioInputChangedEvent
    extends TypedEvent<"audioinputchanged"> {
    public constructor(public readonly audio: MediaDeviceInfo) {
        super("audioinputchanged");
    }
}

export class DeviceManagerVideoInputChangedEvent
    extends TypedEvent<"videoinputchanged"> {
    public constructor(public readonly video: MediaDeviceInfo) {
        super("videoinputchanged");
    }
}

const PREFERRED_AUDIO_OUTPUT_ID_KEY = "calla:preferredAudioOutputID";
const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";
const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";

export class DeviceManager
    extends TypedEventBase<{
        audiooutputchanged: DeviceManagerAudioOutputChangedEvent;
        audioinputchanged: DeviceManagerAudioInputChangedEvent;
        videoinputchanged: DeviceManagerVideoInputChangedEvent;
    }> {

    private _hasAudioPermission = false;
    get hasAudioPermission(): boolean {
        return this._hasAudioPermission;
    }

    private _hasVideoPermission = false;
    get hasVideoPermission(): boolean {
        return this._hasVideoPermission;
    }

    private _currentStream: MediaStream = null;
    get currentStream(): MediaStream {
        return this._currentStream;
    }

    set currentStream(v: MediaStream) {
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

    readonly ready: Promise<void>;

    constructor(private element: HTMLAudioElement, public needsVideoDevice = false) {
        super();

        this.ready = this.start();

        Object.seal(this);
    }

    private async start(): Promise<void> {
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

    get preferredAudioOutputID(): string {
        if (!canChangeAudioOutput) {
            return null;
        }

        return localStorage.getItem(PREFERRED_AUDIO_OUTPUT_ID_KEY);
    }


    get preferredAudioInputID(): string {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }

    get preferredVideoInputID(): string {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }

    async getAudioOutputDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        if (!canChangeAudioOutput) {
            return [];
        }

        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.audioOutput || [];
    }

    async getAudioInputDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.audioInput || [];
    }

    async getVideoInputDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices && devices.videoInput || [];
    }

    async getAudioOutputDevice(): Promise<MediaDeviceInfo> {
        if (!canChangeAudioOutput) {
            return null;
        }

        const curId = this.element && this.element.sinkId;
        if (isNullOrUndefined(curId)) {
            return null;
        }

        const devices = await this.getAudioOutputDevices(),
            device = arrayScan(devices,
                (d) => d.deviceId === curId);

        return device;
    }

    async getAudioInputDevice(): Promise<MediaDeviceInfo> {
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
        const device = arrayScan(devices,
            (d) => testTrack.label === d.label);

        return device;
    }

    async getVideoInputDevice(): Promise<MediaDeviceInfo> {
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
        const device = arrayScan(devices,
            (d) => testTrack.label === d.label);

        return device;
    }

    private async getPreferredAudioOutput(): Promise<MediaDeviceInfo> {
        if (!canChangeAudioOutput) {
            return null;
        }

        const devices = await this.getAudioOutputDevices();
        const device = arrayScan(
            devices,
            (d) => d.deviceId === this.preferredAudioOutputID,
            (d) => d.deviceId === "default",
            (d) => d.deviceId.length > 0);
        return device;
    }

    private async getPreferredAudioInput(): Promise<MediaDeviceInfo> {
        const devices = await this.getAudioInputDevices();
        const device = arrayScan(
            devices,
            (d) => d.deviceId === this.preferredAudioInputID,
            (d) => d.deviceId === "default",
            (d) => d.deviceId.length > 0);
        return device;
    }

    private async getPreferredVideoInput(): Promise<MediaDeviceInfo> {
        const devices = await this.getVideoInputDevices();
        const device = arrayScan(
            devices,
            (d) => d.deviceId === this.preferredVideoInputID,
            (d) => this.needsVideoDevice && d.deviceId.length > 0);
        return device;
    }

    async setAudioOutputDevice(device: MediaDeviceInfo) {
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

    async setAudioInputDevice(device: MediaDeviceInfo) {
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

    async setVideoInputDevice(device: MediaDeviceInfo) {
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

    private async getAvailableDevices(filterDuplicates: boolean = false): Promise<MediaDeviceSet> {
        let devices = await this.getDevices();

        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }

        return {
            audioOutput: canChangeAudioOutput ? devices.filter((d) => d.kind === "audiooutput") : [],
            audioInput: devices.filter((d) => d.kind === "audioinput"),
            videoInput: devices.filter((d) => d.kind === "videoinput")
        };
    }

    private async getDevices(): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
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

        devices = arraySortByKey(devices || [], (d) => d.label);
        return devices;
    }

    private startStream(): Promise<MediaStream> {
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

    async getMediaPermissions(): Promise<MediaPermissionSet> {
        await this.getDevices();
        return {
            audio: this.hasAudioPermission,
            video: this.hasVideoPermission
        };
    }
}