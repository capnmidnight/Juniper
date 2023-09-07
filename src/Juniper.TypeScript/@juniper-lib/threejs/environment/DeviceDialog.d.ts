import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioManager } from "@juniper-lib/audio/AudioManager";
import { DeviceManager } from "@juniper-lib/audio/DeviceManager";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { LocalUserWebcam } from "@juniper-lib/video/LocalUserWebcam";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
export declare class DeviceDialog extends DialogBox {
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
    constructor(fetcher: IFetcher, devices: DeviceManager, audio: AudioManager, microphones: LocalUserMicrophone, webcams: LocalUserWebcam, DEBUG?: boolean);
    get showWebcams(): boolean;
    set showWebcams(v: boolean);
    get showMicrophones(): boolean;
    set showMicrophones(v: boolean);
    protected onShowing(): Promise<void>;
}
//# sourceMappingURL=DeviceDialog.d.ts.map