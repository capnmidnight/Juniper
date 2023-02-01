import { arrayScan, arraySortByKey } from "@juniper-lib/tslib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";
import { StreamChangedEvent } from "./StreamChangedEvent";

export class DeviceSettingsChangedEvent extends TypedEvent<"devicesettingschanged"> {
    constructor() {
        super("devicesettingschanged");
    }
}

export interface IDeviceSource extends TypedEventBase<{
    "streamchanged": StreamChangedEvent;
}> {
    get deviceType(): "audio" | "video";
    get enabled(): boolean;
    get preferredDeviceID(): string;
    get device(): MediaDeviceInfo;
    setDevice(v: MediaDeviceInfo): Promise<void>;
    set outStream(v: MediaStream);
}

export class DeviceManager extends TypedEventBase<{
    "streamchanged": StreamChangedEvent;
}>{
    private _hasPermission = false;
    private initTask = Promise.resolve(0);

    private readonly managers: IDeviceSource[];

    constructor(...managers: IDeviceSource[]) {
        super();
        this.managers = managers;

        const onStreamChanged = (evt: StreamChangedEvent) =>
            this.dispatchEvent(evt);

        for (const manager of this.managers) {
            manager.addScopedEventListener(this, "streamchanged", onStreamChanged);
        }
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
            const anyDevice = arrayScan(devices, dev => dev.label.length > 0);
            if (isDefined(anyDevice)) {
                this._hasPermission = true;
                await Promise.all(this.managers
                    .map(m => m.setDevice(arrayScan(
                        devices,
                        d => d.kind === m.deviceType + "input" && d.deviceId === m.preferredDeviceID,
                        d => d.kind === m.deviceType + "input" && d.deviceId === "default",
                        d => d.kind === m.deviceType + "input" && d.deviceId.length > 0)
                    )));
            }
        }

        return tryCount + 1;
    }

    async getDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();

            if (!this.hasPermission) {
                for (const device of devices) {
                    this._hasPermission ||= device.deviceId.length > 0
                        && device.label.length > 0;

                    if (this.hasPermission) {
                        break;
                    }
                }

                if (!this.hasPermission) {
                    try {
                        const constraints: Partial<Record<"audio" | "video", boolean>> = {};
                        for (const manager of this.managers) {
                            if (manager.enabled) {
                                constraints[manager.deviceType] = true;
                            }
                        }
                        testStream = await navigator.mediaDevices.getUserMedia(constraints);
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

        return devices;
    }

    get outStreams() {
        return this.managers.map(m => m.outStream);
    }
}
