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
import { em, marginLeft, minWidth } from "@juniper-lib/dom/css";
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
import { Audio_Mpeg } from "@juniper-lib/mediatypes";
import { makeLookup } from "@juniper-lib/tslib/collections/makeLookup";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { SetTimeoutTimer } from "@juniper-lib/tslib/timers/SetTimeoutTimer";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
import { InputRangeWithNumber } from "@juniper-lib/widgets/InputRangeWithNumber";
import { group, PropertyList } from "@juniper-lib/widgets/PropertyList";
import type { BaseTele } from "../BaseTele";
import type { Environment } from "./Environment";

const MIC_GROUP = "micFields" + stringRandom(8);

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

    private readonly microphones: HTMLSelectElement;
    private readonly webcams: HTMLSelectElement;
    private readonly micScenario: HTMLMeterElement;
    private readonly activity: ActivityDetector;
    private readonly micVolumeControl: InputRangeWithNumber;
    private readonly spkrVolumeControl: InputRangeWithNumber = null;
    private ready: Promise<void> = null;
    private readonly speakers: HTMLSelectElement = null;
    private readonly properties: PropertyList;
    private readonly testSpkrButton: HTMLButtonElement;
    private readonly useHeadphones: HTMLInputElement;
    private readonly headphoneWarning: HTMLDivElement;

    private readonly timer = new SetTimeoutTimer(30);

    private tele: BaseTele = null;

    constructor(private readonly env: Environment) {
        super("Configure devices");

        this.cancelButton.style.display = "none";

        const clipAsset = new AssetFile("/audio/test-clip.mp3", Audio_Mpeg, !this.env.DEBUG);

        const clipLoaded = this.env.fetcher.assets(clipAsset)
            .then(() => this.env.audio.createBasicClip("test-audio", clipAsset, 0.5));

        elementApply(this.container,
            minWidth("max-content")
        );

        elementApply(this.contentArea,
            this.properties = new PropertyList(
                group(
                    MIC_GROUP,
                    "Input",

                    ["Webcams",
                        this.webcams = Select(
                            onInput(async () => {
                                const deviceId = this.webcams.value;
                                const device = this.camLookup.get(deviceId);
                                await this.env.webcams.setDevice(device);
                            })
                        )],

                    ["Microphones",
                        this.microphones = Select(
                            onInput(async () => {
                                const deviceId = this.microphones.value;
                                const device = this.micLookup.get(deviceId);
                                await this.env.microphones.setDevice(device);
                            })
                        )],

                    ["Input level",
                        this.micScenario = Meter()],

                    ["Volume", this.micVolumeControl = new InputRangeWithNumber(
                        min(0),
                        max(100),
                        step(1),
                        value(0),
                        onInput(() => {
                            env.microphones.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
                        })
                    )],

                    "Output"
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
                            await this.env.audio.speakers.setAudioOutputDevice(device);
                        })
                    )]
            );

            this.env.audio.speakers.addEventListener("audiooutputchanged", (evt) => {
                this.speakers.value = evt.device && evt.device.deviceId || "";
            });
        }

        this.properties.append(
            ["Using headphones",
                this.useHeadphones = InputCheckbox(
                    checked(this.env.audio.useHeadphones),
                    onInput(() => {
                        this.env.audio.useHeadphones = this.useHeadphones.checked;
                        elementSetDisplay(this.headphoneWarning, !this.env.audio.useHeadphones, "inline-block");
                    })
                ),

                this.testSpkrButton = ButtonSecondary(
                    "Test",
                    title("Test audio"),
                    marginLeft(em(0.5)),
                    onClick(async () => {
                        this.testSpkrButton.disabled = true;
                        await clipLoaded;
                        await this.env.audio.playClipThrough("test-audio");
                        this.testSpkrButton.disabled = false;
                    }))],

            this.headphoneWarning = Div(
                className("alert alert-warning"),
                "🎧🎙️ This site has a voice chat feature. Voice chat is best experienced using headphones."
            ),

            ["Volume",
                this.spkrVolumeControl = new InputRangeWithNumber(
                    min(0),
                    max(100),
                    step(1),
                    value(0),
                    onInput(() =>
                        env.audio.destination.volume
                        = this.spkrVolumeControl.valueAsNumber / 100)
                )]
        );

        this.activity = new ActivityDetector(this.env.audio.context);
        this.activity.name = "device-settings-dialog-activity";
        this.env.microphones.connect(this.activity);

        this.timer.addTickHandler(() => {
            this.micScenario.value = this.activity.level;
        });

        this.properties.setGroupVisible(MIC_GROUP, false);

        this.waitForTele();

        Object.seal(this);
    }

    private async waitForTele() {
        this.tele = await this.env.apps.waitFor<BaseTele>("tele");

        this.properties.setGroupVisible(MIC_GROUP, true);
    }

    private async load() {
        await this.env.audio.ready;
    }

    protected override async onShowing(): Promise<void> {
        if (this.ready === null) {
            this.ready = this.load();
        }

        await this.ready;

        if (this.tele) {
            this.env.microphones.usingHeadphones = this.useHeadphones.checked;
            this.env.microphones.enabled = true;
            await this.env.devices.init();

            const devices = await this.env.devices.getDevices();

            const mics = devices.filter(d => d.kind === "audioinput");
            this.micLookup = makeLookup(mics, (m) => m.deviceId);

            const cams = devices.filter(d => d.kind === "videoinput");
            this.camLookup = makeLookup(cams, (m) => m.deviceId);

            makeDeviceSelector(this.microphones, mics, this.env.microphones.device);
            makeDeviceSelector(this.webcams, cams, this.env.webcams.device);

            this.micVolumeControl.valueAsNumber = this.env.microphones.gain.value * 100;
        }

        if (canChangeAudioOutput) {
            await this.env.audio.speakers.ready;
            const spkrs = await this.env.audio.speakers.getAudioOutputDevices();
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

            let curSpker = await this.env.audio.speakers.getAudioOutputDevice();
            if (!curSpker) {
                curSpker = await this.env.audio.speakers.getPreferredAudioOutput();
                await this.env.audio.speakers.setAudioOutputDevice(curSpker);
            }
            this.speakers.value = curSpker && curSpker.deviceId || "";
        }

        this.spkrVolumeControl.valueAsNumber = this.env.audio.destination.volume * 100;

        this.useHeadphones.checked = this.env.audio.useHeadphones;
        elementSetDisplay(this.headphoneWarning, !this.env.audio.useHeadphones, "inline-block");

        this.timer.start();

        await super.onShowing();
    }

    protected override onClosed() {
        this.timer.stop();
        super.onClosed();
    }
}