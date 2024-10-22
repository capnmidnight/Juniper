import { ActivityDetector, AudioManager, DeviceManager, LocalUserMicrophone } from "@juniper-lib/audio";
import { IFetcher } from "@juniper-lib/fetcher";
import { LocalUserWebcamElement } from "@juniper-lib/video";
import { BaseDialogElement } from '../../../widgets/src/BaseDialogElement';
export declare class DeviceDialog extends BaseDialogElement<void> {
    private readonly devices;
    private readonly audio;
    private readonly microphones;
    private readonly webcams;
    private micLookup;
    private camLookup;
    private spkrLookup;
    private readonly microphoneSelector;
    private readonly webcamSelector;
    private readonly micLevels;
    private readonly micVolumeControl;
    private readonly spkrVolumeControl;
    private readonly speakers;
    private readonly properties;
    private readonly testSpkrButton;
    private readonly useHeadphones;
    private readonly headphoneWarning;
    readonly activity: ActivityDetector;
    constructor(fetcher: IFetcher, devices: DeviceManager, audio: AudioManager, microphones: LocalUserMicrophone, webcams: LocalUserWebcamElement, DEBUG?: boolean);
    get showWebcams(): boolean;
    set showWebcams(v: boolean);
    get showMicrophones(): boolean;
    set showMicrophones(v: boolean);
}
//# sourceMappingURL=DeviceDialog.d.ts.map