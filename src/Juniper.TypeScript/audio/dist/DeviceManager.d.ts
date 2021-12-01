import { TypedEvent, TypedEventBase } from "juniper-tslib";
import type { MediaPermissionSet } from "./MediaPermissionSet";
/**
 * Indicates whether or not the current browser can change the destination device for audio output.
 **/
export declare const canChangeAudioOutput: boolean;
export declare class DeviceManagerAudioOutputChangedEvent extends TypedEvent<"audiooutputchanged"> {
    readonly device: MediaDeviceInfo;
    constructor(device: MediaDeviceInfo);
}
export declare class DeviceManagerAudioInputChangedEvent extends TypedEvent<"audioinputchanged"> {
    readonly audio: MediaDeviceInfo;
    constructor(audio: MediaDeviceInfo);
}
export declare class DeviceManagerVideoInputChangedEvent extends TypedEvent<"videoinputchanged"> {
    readonly video: MediaDeviceInfo;
    constructor(video: MediaDeviceInfo);
}
export declare class DeviceManager extends TypedEventBase<{
    audiooutputchanged: DeviceManagerAudioOutputChangedEvent;
    audioinputchanged: DeviceManagerAudioInputChangedEvent;
    videoinputchanged: DeviceManagerVideoInputChangedEvent;
}> {
    private element;
    needsVideoDevice: boolean;
    private _hasAudioPermission;
    get hasAudioPermission(): boolean;
    private _hasVideoPermission;
    get hasVideoPermission(): boolean;
    private _currentStream;
    get currentStream(): MediaStream;
    set currentStream(v: MediaStream);
    readonly ready: Promise<void>;
    constructor(element: HTMLAudioElement, needsVideoDevice?: boolean);
    private start;
    startPreferredAudioInput(): Promise<void>;
    startPreferredVideoInput(): Promise<void>;
    get preferredAudioOutputID(): string;
    get preferredAudioInputID(): string;
    get preferredVideoInputID(): string;
    getAudioOutputDevices(filterDuplicates?: boolean): Promise<MediaDeviceInfo[]>;
    getAudioInputDevices(filterDuplicates?: boolean): Promise<MediaDeviceInfo[]>;
    getVideoInputDevices(filterDuplicates?: boolean): Promise<MediaDeviceInfo[]>;
    getAudioOutputDevice(): Promise<MediaDeviceInfo>;
    getAudioInputDevice(): Promise<MediaDeviceInfo>;
    getVideoInputDevice(): Promise<MediaDeviceInfo>;
    private getPreferredAudioOutput;
    private getPreferredAudioInput;
    private getPreferredVideoInput;
    setAudioOutputDevice(device: MediaDeviceInfo): Promise<void>;
    setAudioInputDevice(device: MediaDeviceInfo): Promise<void>;
    setVideoInputDevice(device: MediaDeviceInfo): Promise<void>;
    private getAvailableDevices;
    private getDevices;
    private startStream;
    getMediaPermissions(): Promise<MediaPermissionSet>;
}
