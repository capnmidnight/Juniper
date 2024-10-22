import { arrayScan, compareBy } from "@juniper-lib/util";
import { TypedEvent } from "@juniper-lib/events";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";
export class DeviceSettingsChangedEvent extends TypedEvent {
    constructor() {
        super("devicesettingschanged");
    }
}
export class DeviceManager {
    constructor(...managers) {
        this.permissed = new Set();
        this.managers = managers;
    }
    get hasPermissions() {
        for (const manager of this.managers) {
            if (!this.permissed.has(manager)) {
                return false;
            }
        }
        return true;
    }
    async init() {
        if (!this.hasPermissions) {
            const devices = await this.getDevices();
            await Promise.all(this.managers
                .map(m => m.setDevice(arrayScan(devices, d => d.kind === m.deviceKind && d.deviceId === m.preferredDeviceID, d => d.kind === m.deviceKind && d.deviceId === "default", d => d.kind === m.deviceKind && d.deviceId.length > 0))));
        }
    }
    async getDevices(filterDuplicates = false) {
        let devices = null;
        let testStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();
            if (!this.hasPermissions) {
                const constraints = {};
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
        devices = (devices || []).sort(deviceComparer);
        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }
        return devices;
    }
    get outStreams() {
        return this.managers.map(m => m.outStream);
    }
}
export const deviceComparer = compareBy(d => d.label);
//# sourceMappingURL=DeviceManager.js.map