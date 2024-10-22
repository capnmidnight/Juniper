import { ITypedEventTarget, TypedEvent } from "@juniper-lib/events";
import { StreamChangedEvent } from "./StreamChangedEvent";
export declare class DeviceSettingsChangedEvent extends TypedEvent<"devicesettingschanged"> {
    constructor();
}
export interface IDeviceSource extends ITypedEventTarget<{
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
export declare class DeviceManager {
    private readonly managers;
    private readonly permissed;
    constructor(...managers: IDeviceSource[]);
    get hasPermissions(): boolean;
    init(): Promise<void>;
    getDevices(filterDuplicates?: boolean): Promise<MediaDeviceInfo[]>;
    get outStreams(): MediaStream[];
}
export declare const deviceComparer: import("@juniper-lib/util").compareCallback<MediaDeviceInfo>;
//# sourceMappingURL=DeviceManager.d.ts.map