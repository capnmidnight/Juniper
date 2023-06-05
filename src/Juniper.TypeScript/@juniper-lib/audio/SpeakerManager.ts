import { arrayScan, arraySortByKey } from "@juniper-lib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/EventBase";
import { IReadyable } from "@juniper-lib/events/IReadyable";
import { Task } from "@juniper-lib/events/Task";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";

/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export const canChangeAudioOutput = /*@__PURE__*/ isFunction(HTMLAudioElement.prototype.setSinkId);

export class AudioOutputChangedEvent
    extends TypedEvent<"audiooutputchanged"> {
    public constructor(public readonly device: MediaDeviceInfo) {
        super("audiooutputchanged");
    }
}

const PREFERRED_AUDIO_OUTPUT_ID_KEY = "calla:preferredAudioOutputID";

export class SpeakerManager
    extends TypedEventBase<{
        audiooutputchanged: AudioOutputChangedEvent;
    }>
    implements IReadyable {

    private _hasAudioPermission = false;
    get hasAudioPermission(): boolean {
        return this._hasAudioPermission;
    }


    private readonly _ready = new Task();

    get ready(): Promise<void> {
        return this._ready;
    }

    get isReady() {
        return this._ready.finished
            && this._ready.resolved;
    }

    constructor(private element: HTMLAudioElement) {
        super();

        this.start();

        Object.seal(this);
    }

    private async start(): Promise<void> {
        if (canChangeAudioOutput) {
            const devices = await navigator.mediaDevices.enumerateDevices();
            const anyDevice = arrayScan(devices, dev => dev.kind === "audiooutput" && dev.label.length > 0);
            if (isDefined(anyDevice)) {
                this._hasAudioPermission = true;
                const device = await this.getPreferredAudioOutput();
                if (device) {
                    await this.setAudioOutputDevice(device);
                }
            }
        }
        this._ready.resolve();
    }

    get preferredAudioOutputID(): string {
        if (!canChangeAudioOutput) {
            return null;
        }

        return localStorage.getItem(PREFERRED_AUDIO_OUTPUT_ID_KEY);
    }

    async getAudioOutputDevices(filterDuplicates = false): Promise<MediaDeviceInfo[]> {
        if (!canChangeAudioOutput) {
            return [];
        }

        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices || [];
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

    async getPreferredAudioOutput(): Promise<MediaDeviceInfo> {
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
                this.dispatchEvent(new AudioOutputChangedEvent(device));
            }
        }
    }

    private async getAvailableDevices(filterDuplicates = false): Promise<MediaDeviceInfo[]> {
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

        return canChangeAudioOutput ? devices.filter((d) => d.kind === "audiooutput") : [];
    }

    private startStream(): Promise<MediaStream> {
        return navigator.mediaDevices.getUserMedia({
            audio: true
        });
    }
}