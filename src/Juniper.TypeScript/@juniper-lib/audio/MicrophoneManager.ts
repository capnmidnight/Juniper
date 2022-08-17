import { arrayScan } from "@juniper-lib/tslib/collections/arrayScan";
import { arraySortByKey } from "@juniper-lib/tslib/collections/arraySortedInsert";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";

export class AudioInputChangedEvent
    extends TypedEvent<"audioinputchanged"> {
    public constructor(public readonly audio: MediaDeviceInfo) {
        super("audioinputchanged");
    }
}
const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";

export class MicrophoneManager
    extends TypedEventBase<{
        audioinputchanged: AudioInputChangedEvent;
    }> {

    private _hasAudioPermission = false;
    get hasAudioPermission(): boolean {
        return this._hasAudioPermission;
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
        const anyDevice = arrayScan(devices, dev => dev.kind === "audioinput" && dev.label.length > 0);
        if (isDefined(anyDevice)) {
            this._hasAudioPermission = true;
            const device = await this.getPreferredAudioInput();
            if (device) {
                await this.setAudioInputDevice(device);
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

    get preferredAudioInputID(): string {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }

    async getAudioInputDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices || [];
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

    private async getPreferredAudioInput(): Promise<MediaDeviceInfo> {
        const devices = await this.getAudioInputDevices();
        const device = arrayScan(
            devices,
            (d) => d.deviceId === this.preferredAudioInputID,
            (d) => d.deviceId === "default",
            (d) => d.deviceId.length > 0);
        return device;
    }

    async setAudioInputDevice(device: MediaDeviceInfo) {
        if (isDefined(device) && device.kind !== "audioinput") {
            throw new Error(`Device is not an audio input device. Was: ${device.kind}. Label: ${device.label}`);
        }

        localStorage.setItem(PREFERRED_AUDIO_INPUT_ID_KEY, device && device.deviceId || null);
        const curAudio = await this.getAudioInputDevice();
        const curAudioID = curAudio && curAudio.deviceId;
        if (this.preferredAudioInputID !== curAudioID) {
            this.dispatchEvent(new AudioInputChangedEvent(device));
        }
    }

    private async getAvailableDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
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
                }
            }

            if (this.hasAudioPermission) {
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

        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }


        return devices.filter((d) => d.kind === "audioinput");
    }

    private startStream(): Promise<MediaStream> {
        return navigator.mediaDevices.getUserMedia({
            audio: this.preferredAudioInputID
                && { deviceId: this.preferredAudioInputID }
                || true
        });
    }
}