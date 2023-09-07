import { DeviceSettingsChangedEvent, IDeviceSource } from "@juniper-lib/audio/DeviceManager";
import { StreamChangedEvent } from "@juniper-lib/audio/StreamChangedEvent";
import { ErsatzElement } from "@juniper-lib/dom/tags";
import { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
export declare class LocalUserWebcam extends TypedEventTarget<{
    devicesettingschanged: DeviceSettingsChangedEvent;
    streamchanged: StreamChangedEvent;
}> implements ErsatzElement<HTMLVideoElement>, IDeviceSource {
    readonly element: HTMLVideoElement;
    private _hasPermission;
    private _device;
    private _enabled;
    constructor();
    get mediaType(): "audio" | "video";
    get deviceKind(): MediaDeviceKind;
    get enabled(): boolean;
    set enabled(v: boolean);
    get hasPermission(): boolean;
    get preferredDeviceID(): string;
    get device(): MediaDeviceInfo;
    checkDevices(devices: MediaDeviceInfo[]): void;
    setDevice(device: MediaDeviceInfo): Promise<void>;
    private onChange;
    get inStream(): MediaStream;
    set inStream(v: MediaStream);
    get outStream(): MediaStream;
    stop(): void;
}
//# sourceMappingURL=LocalUserWebcam.d.ts.map