import { arrayScan, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/util";
import { Task, TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { deviceComparer } from "./DeviceManager";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";
/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export const canChangeAudioOutput = /*@__PURE__*/ isFunction(HTMLAudioElement.prototype.setSinkId);
export class AudioOutputChangedEvent extends TypedEvent {
    constructor(device) {
        super("audiooutputchanged");
        this.device = device;
    }
}
const PREFERRED_AUDIO_OUTPUT_ID_KEY = "calla:preferredAudioOutputID";
export class SpeakerManager extends TypedEventTarget {
    get hasAudioPermission() {
        return this._hasAudioPermission;
    }
    get ready() {
        return this._ready;
    }
    get isReady() {
        return this._ready.finished
            && this._ready.resolved;
    }
    constructor(element) {
        super();
        this.element = element;
        this._hasAudioPermission = false;
        this._ready = new Task();
        this.start();
        Object.seal(this);
    }
    async start() {
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
    get preferredAudioOutputID() {
        if (!canChangeAudioOutput) {
            return null;
        }
        return localStorage.getItem(PREFERRED_AUDIO_OUTPUT_ID_KEY);
    }
    async getAudioOutputDevices(filterDuplicates = false) {
        if (!canChangeAudioOutput) {
            return [];
        }
        const devices = await this.getAvailableDevices(filterDuplicates);
        return devices || [];
    }
    async getAudioOutputDevice() {
        if (!canChangeAudioOutput) {
            return null;
        }
        const curId = this.element && this.element.sinkId;
        if (isNullOrUndefined(curId)) {
            return null;
        }
        const devices = await this.getAudioOutputDevices(), device = arrayScan(devices, (d) => d.deviceId === curId);
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
                this.dispatchEvent(new AudioOutputChangedEvent(device));
            }
        }
    }
    async getAvailableDevices(filterDuplicates = false) {
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
        devices = (devices || []).sort(deviceComparer);
        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }
        return canChangeAudioOutput ? devices.filter((d) => d.kind === "audiooutput") : [];
    }
    startStream() {
        return navigator.mediaDevices.getUserMedia({
            audio: true
        });
    }
}
//# sourceMappingURL=SpeakerManager.js.map