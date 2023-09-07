import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperGainNode } from "./context/JuniperGainNode";
import { DeviceSettingsChangedEvent, IDeviceSource } from "./DeviceManager";
import { StreamChangedEvent } from "./StreamChangedEvent";
export declare class LocalUserMicrophone extends BaseNodeCluster<{
    devicesettingschanged: DeviceSettingsChangedEvent;
    streamchanged: StreamChangedEvent;
}> implements IDeviceSource {
    private localStreamNode;
    private readonly volume;
    readonly autoGainNode: JuniperGainNode;
    private readonly compressor;
    private readonly output;
    private _hasPermission;
    private _usingHeadphones;
    private _device;
    private _enabled;
    constructor(context: JuniperAudioContext);
    get mediaType(): "audio" | "video";
    get deviceKind(): MediaDeviceKind;
    get enabled(): boolean;
    set enabled(v: boolean);
    get hasPermission(): boolean;
    get preferredDeviceID(): string;
    get device(): MediaDeviceInfo;
    setDevice(device: MediaDeviceInfo): Promise<void>;
    get inStream(): MediaStream;
    set inStream(mediaStream: MediaStream);
    get outStream(): MediaStream;
    get gain(): import("./IAudioNode").IAudioParam;
    get muted(): boolean;
    set muted(v: boolean);
    get usingHeadphones(): boolean;
    set usingHeadphones(v: boolean);
    private onChange;
}
//# sourceMappingURL=LocalUserMicrophone.d.ts.map