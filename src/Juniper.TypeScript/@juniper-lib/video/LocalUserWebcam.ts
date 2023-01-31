import { DeviceChangedEvent } from "@juniper-lib/audio/DeviceChangedEvent";
import { filterDeviceDuplicates } from "@juniper-lib/audio/filterDeviceDuplicates";
import { StreamChangedEvent } from "@juniper-lib/audio/StreamChangedEvent";
import { ErsatzElement, Video } from "@juniper-lib/dom/tags";
import { arrayScan, arraySortByKey } from "@juniper-lib/tslib/collections/arrays";
import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";

const PREFERRED_VIDEO_INPUT_ID_KEY = "calla:preferredVideoInputID";

export class LocalUserWebcam
    extends TypedEventBase<{
        devicechanged: DeviceChangedEvent;
        streamchanged: StreamChangedEvent;
    }>
    implements ErsatzElement<HTMLVideoElement> {

    readonly element = Video();

    private initTask = Promise.resolve(0);
    private _hasPermission = false;
    private _device: MediaDeviceInfo = null;

    constructor() {
        super();
        Object.seal(this);
    }

    get hasPermission(): boolean {
        return this._hasPermission;
    }

    init(): Promise<number> {
        return this.initTask = this.initTask.then((i) => this._initInternal(i));
    }

    private async _initInternal(tryCount: number): Promise<number> {
        if (!this.hasPermission) {
            const devices = tryCount === 0
                ? await navigator.mediaDevices.enumerateDevices()
                : await this.getDevices();
            const anyDevice = arrayScan(devices, dev => dev.kind === "videoinput" && dev.label.length > 0);
            if (isDefined(anyDevice)) {
                this._hasPermission = true;
                this._device = arrayScan(
                    devices,
                    (d) => d.deviceId === this.preferredDeviceID,
                    (d) => d.deviceId === "default",
                    (d) => d.deviceId.length > 0);
            }
        }

        return tryCount + 1;
    }

    get preferredDeviceID(): string {
        return localStorage.getItem(PREFERRED_VIDEO_INPUT_ID_KEY);
    }

    async getDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();

            if (!this.hasPermission) {
                for (const device of devices) {
                    this._hasPermission ||= device.kind === "videoinput"
                        && device.deviceId.length > 0
                        && device.label.length > 0;

                    if (this.hasPermission) {
                        break;
                    }
                }

                if (!this.hasPermission) {
                    try {
                        testStream = await navigator.mediaDevices.getUserMedia({
                            video: true
                        });
                    }
                    catch (exp) {
                        console.warn(exp);
                    }
                }
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

        return devices.filter((d) => d.kind === "videoinput");
    }

    get device() {
        return this._device;
    }

    async setDevice(device: MediaDeviceInfo) {
        if (isDefined(device) && device.kind !== "videoinput") {
            throw new Error(`Device is not an vide input device. Was: ${device.kind}. Label: ${device.label}`);
        }

        const curVideoID = this.device && this.device.deviceId || null;
        const nextVideoID = device && device.deviceId || null;
        if (nextVideoID !== curVideoID) {
            this._device = device;
            localStorage.setItem(PREFERRED_VIDEO_INPUT_ID_KEY, nextVideoID);
            this.dispatchEvent(new DeviceChangedEvent(device));
            if (this.stream) {
                await this.start();
            }
        }
    }

    get stream(): MediaStream {
        return this.element.srcObject as MediaStream;
    }

    set stream(v: MediaStream) {
        if (v !== this.stream) {
            const oldStream = this.stream;

            if (this.stream
                && this.stream.active) {
                for (const track of this.stream.getTracks()) {
                    track.stop();
                }
            }

            this.element.srcObject = v;

            if (this.stream) {
                this.element.play();
            }

            this.dispatchEvent(new StreamChangedEvent(this.device, oldStream, this.stream));
        }
    }

    get muted(): boolean {
        return !!this.stream;
    }

    set muted(v: boolean) {
        if (v !== this.muted) {
            if (v) {
                this.start();
            }
            else {
                this.stop();
            }
        }
    }

    async start(): Promise<void> {
        if (this.device) {
            this.stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    deviceId: this.device.deviceId
                }
            });
        }
    }

    stop() {
        this.stream = null;
    }
}