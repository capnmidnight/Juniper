import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioManager } from "@juniper-lib/audio/AudioManager";
import { DeviceManager } from "@juniper-lib/audio/DeviceManager";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { canChangeAudioOutput } from "@juniper-lib/audio/SpeakerManager";
import {
    checked,
    className,
    max,
    min,
    selected,
    step,
    title,
    value
} from "@juniper-lib/dom/attrs";
import { display, em, margin, paddingRight } from "@juniper-lib/dom/css";
import { onClick, onInput } from "@juniper-lib/dom/evts";
import {
    ButtonSecondary, Div,
    elementApply,
    elementClearChildren,
    elementSetDisplay,
    InputCheckbox,
    Meter,
    Option,
    Select
} from "@juniper-lib/dom/tags";
import { AssetFile } from "@juniper-lib/fetcher/Asset";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { makeLookup } from "@juniper-lib/tslib/collections/makeLookup";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { LocalUserWebcam } from "@juniper-lib/video/LocalUserWebcam";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
import { InputRangeWithNumber } from "@juniper-lib/widgets/InputRangeWithNumber";
import { group, PropertyList } from "@juniper-lib/widgets/PropertyList";

const MIC_GROUP = "micFields" + stringRandom(8);
const CAM_GROUP = "camFields" + stringRandom(8);

function makeDeviceSelector(selector: HTMLSelectElement, devices: MediaDeviceInfo[], curDevice: MediaDeviceInfo) {
    elementClearChildren(selector);
    elementApply(selector,
        Option(value(""), "NONE"),
        ...devices.map((device) =>
            Option(
                selected(curDevice
                    && device.deviceId === curDevice.deviceId),
                value(device.deviceId),
                device.label
            )
        )
    );
}

export class DeviceDialog extends DialogBox {
    private micLookup: Map<string, MediaDeviceInfo> = null;
    private camLookup: Map<string, MediaDeviceInfo> = null;
    private spkrLookup: Map<string, MediaDeviceInfo> = null;

    private readonly microphoneSelector: HTMLSelectElement;
    private readonly webcamSelector: HTMLSelectElement;
    private readonly micLevels: HTMLMeterElement;
    private readonly micVolumeControl: InputRangeWithNumber;
    private readonly spkrVolumeControl: InputRangeWithNumber = null;
    private readonly speakers: HTMLSelectElement = null;
    private readonly properties: PropertyList;
    private readonly testSpkrButton: HTMLButtonElement;
    private readonly useHeadphones: HTMLInputElement;
    private readonly headphoneWarning: HTMLDivElement;

    readonly activity: ActivityDetector;

    constructor(fetcher: IFetcher,
        private readonly devices: DeviceManager,
        private readonly audio: AudioManager,
        private readonly microphones: LocalUserMicrophone,
        private readonly webcams: LocalUserWebcam,
        DEBUG = false) {
        super("Configure devices");

        this.cancelButton.style.display = "none";

        const clipAsset = new AssetFile("/audio/test-clip.mp3", Audio_Mpeg, !DEBUG);

        const clipLoaded = fetcher.assets(clipAsset)
            .then(() => this.audio.createBasicClip("test-audio", clipAsset, 0.5));

        elementApply(this.contentArea,
            paddingRight("2em"),
            this.properties = new PropertyList(
                group(CAM_GROUP,
                    ["Webcams",
                        this.webcamSelector = Select(
                            onInput(async () => {
                                const deviceId = this.webcamSelector.value;
                                const device = this.camLookup.get(deviceId);
                                await this.webcams.setDevice(device);
                            })
                        )]
                ),
                group(
                    MIC_GROUP,

                    ["Microphones",
                        this.microphoneSelector = Select(
                            display("inline-block"),
                            onInput(async () => {
                                const deviceId = this.microphoneSelector.value;
                                const device = this.micLookup.get(deviceId);
                                await this.microphones.setDevice(device);
                            })
                        )],

                    ["Volume",
                        this.micVolumeControl = new InputRangeWithNumber(
                            min(0),
                            max(100),
                            step(1),
                            value(0),
                            onInput(() => {
                                this.microphones.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
                            })
                        )],

                    ["Levels",
                        this.micLevels = Meter(
                            display("inline-block")
                        )]
                )
            )
        );

        if (canChangeAudioOutput) {
            this.properties.append(
                ["Speakers",
                    this.speakers = Select(
                        onInput(async () => {
                            const deviceId = this.speakers.value;
                            const device = this.spkrLookup.get(deviceId);
                            await this.audio.speakers.setAudioOutputDevice(device);
                        })
                    )]
            );

            this.audio.speakers.addEventListener("audiooutputchanged", (evt) => {
                this.speakers.value = evt.device && evt.device.deviceId || "";
            });
        }

        this.properties.append(
            ["Volume",
                this.spkrVolumeControl = new InputRangeWithNumber(
                    min(0),
                    max(100),
                    step(1),
                    value(0),
                    onInput(() =>
                        this.audio.destination.volume
                        = this.spkrVolumeControl.valueAsNumber / 100)
                )],

            ["",
                this.testSpkrButton = ButtonSecondary(
                    "Test",
                    title("Test audio"),
                    margin(em(0.5)),
                    onClick(async () => {
                        this.testSpkrButton.disabled = true;
                        await clipLoaded;
                        await this.audio.playClipThrough("test-audio");
                        this.testSpkrButton.disabled = false;
                    }))],

            ["Using headphones",
                this.useHeadphones = InputCheckbox(
                    checked(this.audio.useHeadphones),
                    onInput(() => {
                        this.audio.useHeadphones = this.useHeadphones.checked;
                        elementSetDisplay(this.headphoneWarning, !this.audio.useHeadphones, "inline-block");
                    })
                )],

            this.headphoneWarning = Div(
                className("alert alert-warning"),
                "🎧🎙️ This site has a voice chat feature. Voice chat is best experienced using headphones."
            )
        );

        this.activity = new ActivityDetector(this.audio.context);
        this.activity.name = "device-settings-dialog-activity";
        this.activity.addEventListener("activity", (evt) => {
            if (this.isOpen) {
                this.micLevels.value = evt.level;
            }
        });
        this.microphones.connect(this.activity);
        this.activity.start();

        this.properties.setGroupVisible(MIC_GROUP, false);

        Object.seal(this);
    }

    get showWebcams(): boolean {
        return this.properties.getGroupVisible(CAM_GROUP);
    }

    set showWebcams(v: boolean) {
        this.properties.setGroupVisible(CAM_GROUP, v);
    }

    get showMicrophones(): boolean {
        return this.properties.getGroupVisible(MIC_GROUP);
    }

    set showMicrophones(v: boolean) {
        this.properties.setGroupVisible(MIC_GROUP, v);
    }

    protected override async onShowing(): Promise<void> {
        if (this.showWebcams || this.showMicrophones) {
            await this.devices.init();

            const devices = await this.devices.getDevices();

            if (this.showWebcams) {
                const cams = devices.filter(d => d.kind === "videoinput");
                this.camLookup = makeLookup(cams, (m) => m.deviceId);
                makeDeviceSelector(this.webcamSelector, cams, this.webcams.device);
            }

            if (this.showMicrophones) {
                const mics = devices.filter(d => d.kind === "audioinput");
                this.micLookup = makeLookup(mics, (m) => m.deviceId);
                makeDeviceSelector(this.microphoneSelector, mics, this.microphones.device);

                this.microphones.usingHeadphones = this.useHeadphones.checked;
                this.microphones.enabled = true;

                this.micVolumeControl.valueAsNumber = this.microphones.gain.value * 100;
            }
        }

        if (canChangeAudioOutput) {
            await this.audio.speakers.ready;
            const spkrs = await this.audio.speakers.getAudioOutputDevices();
            this.spkrLookup = makeLookup(spkrs, (device) => device.deviceId);

            elementClearChildren(this.speakers);
            elementApply(this.speakers,
                ...spkrs.map((device) =>
                    Option(
                        value(device.deviceId),
                        device.label
                    )
                )
            );

            let curSpker = await this.audio.speakers.getAudioOutputDevice();
            if (!curSpker) {
                curSpker = await this.audio.speakers.getPreferredAudioOutput();
                await this.audio.speakers.setAudioOutputDevice(curSpker);
            }
            this.speakers.value = curSpker && curSpker.deviceId || "";
        }

        this.spkrVolumeControl.valueAsNumber = this.audio.destination.volume * 100;

        this.useHeadphones.checked = this.audio.useHeadphones;
        elementSetDisplay(this.headphoneWarning, !this.audio.useHeadphones, "inline-block");

        await super.onShowing();
    }
}