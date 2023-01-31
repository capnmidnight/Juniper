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
import { all } from "@juniper-lib/tslib/events/all";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { SetTimeoutTimer } from "@juniper-lib/tslib/timers/SetTimeoutTimer";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
import { InputRangeWithNumber } from "@juniper-lib/widgets/InputRangeWithNumber";
import { group, PropertyList } from "@juniper-lib/widgets/PropertyList";
import type { BaseTele } from "../BaseTele";
import type { Environment } from "./Environment";

const MIC_GROUP = "micFields" + stringRandom(8);

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
                                await this.mic.setDevice(device);
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
                            env.audio.localMic.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
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

                this.testSpkrButton = ButtonSecondary("Test",
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
        this.mic.connect(this.activity);

        this.mic.addEventListener("devicechanged", (evt) => {
            this.microphones.value = evt.device && evt.device.deviceId || "";
        });

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

    private get mic() {
        return this.env.audio.localMic;
    }

    protected override async onShowing(): Promise<void> {
        if (this.ready === null) {
            this.ready = this.load();
        }

        await this.ready;

        if (this.tele) {
            await all(
                this.mic.init(),
                this.env.webcams.init()
            );
            this.mic.usingHeadphones = this.useHeadphones.checked;
            await this.mic.start();

            const mics = await this.mic.getDevices();
            this.micLookup = makeLookup(mics, (m) => m.deviceId);

            const cams = await this.env.webcams.getDevices();
            this.camLookup = makeLookup(cams, (m) => m.deviceId);

            elementClearChildren(this.microphones);
            elementApply(this.microphones,
                Option(value(""), "NONE"),
                ...mics.map((device) =>
                    Option(
                        selected(this.mic.device
                            && device.deviceId === this.mic.device.deviceId),
                        value(device.deviceId),
                        device.label
                    )
                )
            );

            elementClearChildren(this.webcams);
            elementApply(this.webcams,
                Option(value(""), "NONE"),
                ...cams.map((device) =>
                    Option(
                        value(device.deviceId),
                        device.label
                    )
                )
            );

            this.microphones.value = this.mic.preferredDeviceID || "";
            this.micVolumeControl.valueAsNumber = this.mic.gain.value * 100;

            this.webcams.value = this.env.webcams.preferredDeviceID || "";
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