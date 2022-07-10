import { arrayScan, arraySortByKey, isDefined, isNullOrUndefined, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";

export class DeviceManagerVideoInputChangedEvent
    extends TypedEvent<"videoinputchanged"> {
    public constructor(public readonly video: MediaDeviceInfo) {
        super("videoinputchanged");
    }
}

const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";

export class CameraManager
    extends TypedEventBase<{
        videoinputchanged: DeviceManagerVideoInputChangedEvent;
    }> {

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

    constructor() {
        super();

        this.ready = this.start();

        Object.seal(this);
    }

    private async start(): Promise<void> {
        const devices = await navigator.mediaDevices.enumerateDevices();
        const anyDevice = arrayScan(devices, dev => dev.kind === "videoinput" && dev.label.length > 0);
        if (isDefined(anyDevice)) {
            this._hasVideoPermission = true;
            const device = await this.getPreferredVideoInput();
            if (device) {
                await this.setVideoInputDevice(device);
            }
        }
    }

    async startPreferredVideoInput() {
        const device = await this.getPreferredVideoInput();
        if (device) {
            await this.setVideoInputDevice(device);
            this.currentStream = await this.startStream();
        }
    }

    get preferredVideoInputID(): string {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }

    async getVideoInputDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices || [];
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

    private async getPreferredVideoInput(): Promise<MediaDeviceInfo> {
        const devices = await this.getVideoInputDevices();
        const device = arrayScan(
            devices,
            (d) => d.deviceId === this.preferredVideoInputID,
            (d) => d.deviceId.length > 0);
        return device;
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

    private async getAvailableDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        let devices = await this.getDevices();

        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }

        return devices.filter((d) => d.kind === "videoinput");
    }

    private async getDevices(): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();
            for (const device of devices) {
                if (device.deviceId.length > 0) {
                    if (!this.hasVideoPermission) {
                        this._hasVideoPermission = device.kind === "videoinput"
                            && device.label.length > 0;
                    }
                }
            }

            if (this.hasVideoPermission) {
                break;
            }

            try {
                testStream = await this.startStream();
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
            video: this.preferredVideoInputID
                && { deviceId: this.preferredVideoInputID }
                || true
        });
    }
}