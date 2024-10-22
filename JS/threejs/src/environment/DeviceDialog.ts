import { makeLookup, stringRandom } from "@juniper-lib/util";
import { ActivityDetector, AudioManager, DeviceManager, LocalUserMicrophone, canChangeAudioOutput } from "@juniper-lib/audio";
import {
    Button,
    Checked,
    ClassList,
    Clear,
    Div,
    For,
    H1,
    HtmlRender,
    ID,
    InputCheckbox,
    Label,
    Max,
    Meter,
    Min,
    OnClick,
    OnInput,
    Option,
    Select,
    Selected,
    SlotAttr,
    Step,
    TitleAttr,
    Value,
    display,
    elementSetDisplay,
    em, margin,
    paddingRight
} from "@juniper-lib/dom";
import { AssetFile, IFetcher } from "@juniper-lib/fetcher";
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { LocalUserWebcamElement } from "@juniper-lib/video";
import { InputRangeWithNumber, InputRangeWithNumberElement, PropertyGroup, PropertyList, PropertyListElement } from "@juniper-lib/widgets";
import { BaseDialogElement } from '../../../widgets/src/BaseDialogElement';

const MIC_GROUP = "micFields" + stringRandom(8);
const CAM_GROUP = "camFields" + stringRandom(8);

function makeDeviceSelector(selector: HTMLSelectElement, devices: MediaDeviceInfo[], curDevice: MediaDeviceInfo) {
    HtmlRender(selector,
        Clear(),
        Option(Value(""), "NONE"),
        ...devices.map((device) =>
            Option(
                Selected(curDevice
                    && device.deviceId === curDevice.deviceId),
                Value(device.deviceId),
                device.label
            )
        )
    );
}

export class DeviceDialog extends BaseDialogElement<void> {
    private micLookup: Map<string, MediaDeviceInfo> = null;
    private camLookup: Map<string, MediaDeviceInfo> = null;
    private spkrLookup: Map<string, MediaDeviceInfo> = null;

    private readonly microphoneSelector: HTMLSelectElement;
    private readonly webcamSelector: HTMLSelectElement;
    private readonly micLevels: HTMLMeterElement;
    private readonly micVolumeControl: InputRangeWithNumberElement;
    private readonly spkrVolumeControl: InputRangeWithNumberElement = null;
    private readonly speakers: HTMLSelectElement = null;
    private readonly properties: PropertyListElement;
    private readonly testSpkrButton: HTMLButtonElement;
    private readonly useHeadphones: HTMLInputElement;
    private readonly headphoneWarning: HTMLDivElement;

    readonly activity: ActivityDetector;

    constructor(fetcher: IFetcher,
        private readonly devices: DeviceManager,
        private readonly audio: AudioManager,
        private readonly microphones: LocalUserMicrophone,
        private readonly webcams: LocalUserWebcamElement,
        DEBUG = false) {

        const id = stringRandom(12);

        const webcamSelector = Select(
            ID(id + "Webcams"),
            OnInput(async () => {
                const deviceId = this.webcamSelector.value;
                const device = this.camLookup.get(deviceId);
                await this.webcams.setDevice(device);
            })
        );

        const microphoneSelector = Select(
            ID(id + "Microphones"),
            display("inline-block"),
            OnInput(async () => {
                const deviceId = this.microphoneSelector.value;
                const device = this.micLookup.get(deviceId);
                await this.microphones.setDevice(device);
            })
        );

        const micVolumeControl = InputRangeWithNumber(
            ID(id + "MicVolume"),
            Min(0),
            Max(100),
            Step(1),
            Value(0),
            OnInput(() => {
                this.microphones.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
            })
        );

        const micLevels = Meter(
            ID(id + "Levels"),
            display("inline-block")
        );

        const properties = PropertyList(
            SlotAttr("modal-body"),

            PropertyGroup(CAM_GROUP,
                Label(For(id + "Webcams"), "Webcams"),
                webcamSelector
            ),
            PropertyGroup(
                MIC_GROUP,
                Label(For(id + "Microphones"), "Microphones"),
                microphoneSelector,

                Label(For(id + "MicVolume"), "Volume"),
                micVolumeControl,

                Label(For(id + "Levels"), "Levels"),
                micLevels
            )
        );

        super(
            H1("Devices"),
            properties
        );

        this.webcamSelector = webcamSelector;
        this.microphoneSelector = microphoneSelector;
        this.micVolumeControl = micVolumeControl;
        this.micLevels = micLevels;
        this.properties = properties;

        const clipAsset = new AssetFile("/audio/test-clip.mp3", Audio_Mpeg, !DEBUG);

        const clipLoaded = fetcher.assets(clipAsset)
            .then(() => this.audio.createBasicClip("test-audio", clipAsset, 0.5));

        HtmlRender(this.body,
            paddingRight("2em"),
            this.properties
        );

        if (canChangeAudioOutput) {
            this.properties.append(
                Label(For(id + "Speackers"), "Speakers"),
                this.speakers = Select(
                    ID(id + "Speakers"),
                    OnInput(async () => {
                        const deviceId = this.speakers.value;
                        const device = this.spkrLookup.get(deviceId);
                        await this.audio.speakers.setAudioOutputDevice(device);
                    })
                )
            );

            this.audio.speakers.addEventListener("audiooutputchanged", (evt) => {
                this.speakers.value = evt.device && evt.device.deviceId || "";
            });
        }

        this.properties.append(
            Label(For(id + "SpkrVolume"), "Volume"),
            this.spkrVolumeControl = InputRangeWithNumber(
                ID(id + "SpkrVolume"),
                Min(0),
                Max(100),
                Step(1),
                Value(0),
                OnInput(() =>
                    this.audio.destination.volume
                    = this.spkrVolumeControl.valueAsNumber / 100)
            ),

            this.testSpkrButton = Button(
                "Test",
                TitleAttr("Test audio"),
                margin(em(0.5)),
                OnClick(async () => {
                    this.testSpkrButton.disabled = true;
                    await clipLoaded;
                    await this.audio.playClipThrough("test-audio");
                    this.testSpkrButton.disabled = false;
                })
            ),

            Label(For(id + "Headphones"), "Using headphones"),
            this.useHeadphones = InputCheckbox(
                ID(id + "Headphones"),
                Checked(this.audio.useHeadphones),
                OnInput(() => {
                    this.audio.useHeadphones = this.useHeadphones.checked;
                    elementSetDisplay(this.headphoneWarning, !this.audio.useHeadphones, "inline-block");
                })
            ),

            this.headphoneWarning = Div(
                ClassList("alert", "alert-warning"),
                "🎧🎙️ This site has a voice chat feature. Voice chat is best experienced using headphones."
            )
        );

        this.activity = new ActivityDetector(this.audio.context);
        this.activity.name = "device-settings-dialog-activity";
        this.activity.addEventListener("activity", (evt) => {
            if (this.open) {
                this.micLevels.value = evt.level;
            }
        });
        this.microphones.connect(this.activity);
        this.activity.start();

        this.properties.setGroupVisible(MIC_GROUP, false);

        this.dialog.addEventListener("showing", async (evt) => {
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

                HtmlRender(this.speakers,
                    Clear(),
                    ...spkrs.map((device) =>
                        Option(
                            Value(device.deviceId),
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
            evt.resolve();
        });

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
}