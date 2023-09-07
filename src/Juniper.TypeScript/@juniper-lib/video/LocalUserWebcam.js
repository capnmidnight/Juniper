import { DeviceSettingsChangedEvent } from "@juniper-lib/audio/DeviceManager";
import { StreamChangedEvent } from "@juniper-lib/audio/StreamChangedEvent";
import { Muted } from "@juniper-lib/dom/attrs";
import { Video } from "@juniper-lib/dom/tags";
import { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";
export class LocalUserWebcam extends TypedEventTarget {
    constructor() {
        super();
        this.element = Video(Muted(true));
        this._hasPermission = false;
        this._device = null;
        this._enabled = false;
        Object.seal(this);
    }
    get mediaType() {
        return "video";
    }
    get deviceKind() {
        return `${this.mediaType}input`;
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        if (v !== this.enabled) {
            this._enabled = v;
            this.onChange();
        }
    }
    get hasPermission() {
        return this._hasPermission;
    }
    get preferredDeviceID() {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }
    get device() {
        return this._device;
    }
    checkDevices(devices) {
        if (!this.hasPermission) {
            for (const device of devices) {
                if (device.kind === this.deviceKind
                    && device.deviceId.length > 0
                    && device.label.length > 0) {
                    this._hasPermission = true;
                    break;
                }
            }
        }
    }
    async setDevice(device) {
        if (isDefined(device) && device.kind !== this.deviceKind) {
            throw new Error(`Device is not an vide input device. Was: ${device.kind}. Label: ${device.label}`);
        }
        const curVideoID = this.device && this.device.deviceId || null;
        const nextVideoID = device && device.deviceId || null;
        if (nextVideoID !== curVideoID) {
            this._device = device;
            localStorage.setItem(PREFERRED_VIDEO_INPUT_ID_KEY, nextVideoID);
            await this.onChange();
        }
    }
    async onChange() {
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
    get inStream() {
        return this.element.srcObject;
    }
    set inStream(v) {
        if (v !== this.inStream) {
            if (this.inStream) {
                this.element.pause();
            }
            this.element.srcObject = v;
            if (this.inStream) {
                this.element.play();
            }
        }
    }
    get outStream() {
        return this.inStream;
    }
    stop() {
        this.inStream = null;
    }
}
//# sourceMappingURL=LocalUserWebcam.js.map