import { DeviceSettingsChangedEvent, IDeviceSource } from "@juniper-lib/audio/DeviceManager";
import { StreamChangedEvent } from "@juniper-lib/audio/StreamChangedEvent";
import { muted } from "@juniper-lib/dom/attrs";
import { ErsatzElement, Video } from "@juniper-lib/dom/tags";
import { TypedEventBase } from "@juniper-lib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";

const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";

export class LocalUserWebcam
    extends TypedEventBase<{
        devicesettingschanged: DeviceSettingsChangedEvent;
        streamchanged: StreamChangedEvent;
    }>
    implements ErsatzElement<HTMLVideoElement>, IDeviceSource {

    readonly element = Video(muted(true));

    private _hasPermission = false;
    private _device: MediaDeviceInfo = null;
    private _enabled = false;

    constructor() {
        super();
        Object.seal(this);
    }

    get mediaType(): "audio" | "video" {
        return "video";
    }

    get deviceKind(): MediaDeviceKind {
        return `${this.mediaType}input`;
    }

    get enabled(): boolean {
        return this._enabled;
    }

    set enabled(v: boolean) {
        if (v !== this.enabled) {
            this._enabled = v;
            this.onChange();
        }
    }

    get hasPermission(): boolean {
        return this._hasPermission;
    }

    get preferredDeviceID(): string {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }

    get device() {
        return this._device;
    }

    checkDevices(devices: MediaDeviceInfo[]): void {
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

    async setDevice(device: MediaDeviceInfo) {
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
        return this.element.srcObject as MediaStream;
    }

    set inStream(v: MediaStream) {
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