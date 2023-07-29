import { arrayScan, arraySortByKey } from "@juniper-lib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/TypedEventBase";
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
    get mediaType(): "audio" | "video";
    get deviceKind(): MediaDeviceKind;
    get enabled(): boolean;
    get preferredDeviceID(): string;
    get device(): MediaDeviceInfo;
    setDevice(v: MediaDeviceInfo): Promise<void>;
    set outStream(v: MediaStream);
}

export class DeviceManager {
    private readonly managers: IDeviceSource[];
    private readonly permissed = new Set<IDeviceSource>();

    constructor(...managers: IDeviceSource[]) {
        this.managers = managers;
    }

    get hasPermissions(): boolean {
        for (const manager of this.managers) {
            if (!this.permissed.has(manager)) {
                return false;
            }
        }

        return true;
    }

    async init(): Promise<void> {
        if (!this.hasPermissions) {
            const devices = await this.getDevices();
            await Promise.all(this.managers
                .map(m =>
                    m.setDevice(arrayScan(
                        devices,
                        d => d.kind === m.deviceKind && d.deviceId === m.preferredDeviceID,
                        d => d.kind === m.deviceKind && d.deviceId === "default",
                        d => d.kind === m.deviceKind && d.deviceId.length > 0)
                    )));
        }
    }

    async getDevices(filterDuplicates = false): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();
            if (!this.hasPermissions) {
                const constraints: Partial<Record<"audio" | "video", boolean>> = {};
                for (const manager of this.managers) {
                    if (!this.permissed.has(manager)) {
                        for (const device of devices) {
                            if (device.kind === manager.deviceKind
                                && device.deviceId.length > 0
                                && device.label.length > 0) {
                                this.permissed.add(manager);
                                break;
                            }
                        }

                        if (!this.permissed.has(manager)) {
                            constraints[manager.mediaType] = true;
                        }
                    }
                }

                if (!this.hasPermissions) {
                    try {
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
