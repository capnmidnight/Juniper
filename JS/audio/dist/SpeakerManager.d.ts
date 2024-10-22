import { IReadyable, TypedEvent, TypedEventTarget } from "@juniper-lib/events";
/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export declare const canChangeAudioOutput: boolean;
export declare class AudioOutputChangedEvent extends TypedEvent<"audiooutputchanged"> {
    readonly device: MediaDeviceInfo;
    constructor(device: MediaDeviceInfo);
}
export declare class SpeakerManager extends TypedEventTarget<{
    audiooutputchanged: AudioOutputChangedEvent;
}> implements IReadyable {
    private element;
    private _hasAudioPermission;
    get hasAudioPermission(): boolean;
    private readonly _ready;
    get ready(): Promise<void>;
    get isReady(): boolean;
    constructor(element: HTMLAudioElement);
    private start;
    get preferredAudioOutputID(): string;
    getAudioOutputDevices(filterDuplicates?: boolean): Promise<MediaDeviceInfo[]>;
    getAudioOutputDevice(): Promise<MediaDeviceInfo>;
    getPreferredAudioOutput(): Promise<MediaDeviceInfo>;
    setAudioOutputDevice(device: MediaDeviceInfo): Promise<void>;
    private getAvailableDevices;
    private startStream;
}
//# sourceMappingURL=SpeakerManager.d.ts.map