import { connect } from "@juniper-lib/audio/nodes";
import { canChangeAudioOutput } from "@juniper-lib/audio/SpeakerManager";
import {
    className,
    max,
    min,
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

export class DeviceDialog extends DialogBox {
    private micLookup: Map<string, MediaDeviceInfo> = null;
    private spkrLookup: Map<string, MediaDeviceInfo> = null;

    private readonly microphones: HTMLSelectElement;
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

        const clipLoaded = this.env.audio.loadBasicClip("test-audio", "/audio/test-clip.mp3", 0.5);

        elementApply(this.container,
            minWidth("max-content")
        );

        elementApply(this.contentArea,
            this.properties = new PropertyList(
                group(
                    MIC_GROUP,
                    "Input",

                    ["Microphones",
                        this.microphones = Select(
                            onInput(async () => {
                                const tele = this.env.apps.get<BaseTele>("tele");
                                const deviceId = this.microphones.value;
                                const device = this.micLookup.get(deviceId);
                                await tele.conference.microphones.setAudioInputDevice(device);
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
                            env.audio.input.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
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
                        env.audio.audioDestination.volume
                        = this.spkrVolumeControl.valueAsNumber / 100)
                )]
        );

        this.activity = new ActivityDetector("device-settings-dialog-activity", this.env.audio.audioCtx);

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

        this.tele.conference.microphones.addEventListener("audioinputchanged", (evt) => {
            this.microphones.value = evt.audio && evt.audio.deviceId || "";
        });

        connect(this.env.audio.input, this.activity);
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
            await this.tele.ready;
            await this.tele.conference.microphones.ready;

            const mics = await this.tele.conference.microphones.getAudioInputDevices();
            this.micLookup = makeLookup(mics, (m) => m.deviceId);

            elementClearChildren(this.microphones);
            elementApply(this.microphones,
                Option(value(""), "NONE"),
                ...mics.map((device) =>
                    Option(
                        value(device.deviceId),
                        device.label
                    )
                )
            )

            const curMic = await this.tele.conference.microphones.getAudioInputDevice();
            this.microphones.value = curMic && curMic.deviceId || "";
            this.micVolumeControl.valueAsNumber = this.env.audio.input.gain.value * 100;
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

        this.spkrVolumeControl.valueAsNumber = this.env.audio.audioDestination.volume * 100;

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