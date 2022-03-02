import { canChangeAudioOutput } from "juniper-audio/DeviceManager";
import { connect } from "juniper-audio/nodes";
import {
    className,
    max,
    min,
    step,
    title,
    value
} from "juniper-dom/attrs";
import { buttonSetEnabled } from "juniper-dom/buttonSetEnabled";
import { marginLeft, minWidth, styles } from "juniper-dom/css";
import { DialogBox } from "juniper-dom/DialogBox";
import { onClick, onInput } from "juniper-dom/evts";
import { InputRangeWithNumber, InputRangeWithNumberElement } from "juniper-dom/InputRangeWithNumber";
import { group, PropertyList } from "juniper-dom/PropertyList";
import {
    Button,
    Div,
    elementApply,
    elementSetDisplay,
    InputCheckbox,
    Meter,
    Option,
    Select
} from "juniper-dom/tags";
import { SetTimeoutTimer } from "juniper-timers";
import { makeLookup, stringRandom } from "juniper-tslib";
import { ActivityDetector } from "juniper-webrtc/ActivityDetector";
import type { Environment } from "./Environment";

const MIC_GROUP = "micFields" + stringRandom(8);

export class DeviceDialog extends DialogBox {
    private micLookup: Map<string, MediaDeviceInfo> = null;
    private spkrLookup: Map<string, MediaDeviceInfo> = null;

    private readonly microphones: HTMLSelectElement;
    private readonly micScenario: HTMLMeterElement;
    private readonly activity: ActivityDetector;
    private readonly micVolumeControl: InputRangeWithNumberElement;
    private readonly spkrVolumeControl: InputRangeWithNumberElement = null;
    private ready: Promise<void> = null;
    private readonly speakers: HTMLSelectElement = null;
    private readonly properties: PropertyList;
    private readonly testSpkrButton: HTMLButtonElement;
    private readonly useHeadphones: HTMLInputElement;
    private readonly headphoneWarning: HTMLDivElement;

    private readonly timer = new SetTimeoutTimer(30);

    constructor(private readonly env: Environment) {
        super("Configure devices");

        this.cancelButton.style.display = "none";

        const clipLoaded = this.env.audio.createBasicClip("test-audio", "/audio/test-clip.mp3", 0.5);

        elementApply(this.container,
            styles(
                minWidth("max-content")));

        elementApply(this.contentArea,
            this.properties = new PropertyList(
                group(
                    MIC_GROUP,
                    "Input",

                    ["Microphones",
                        this.microphones = Select(
                            onInput(async () => {
                                const deviceId = this.microphones.value;
                                const device = this.micLookup.get(deviceId);
                                await this.env.audio.devices.setAudioInputDevice(device);
                            })
                        )],

                    ["Input level",
                        this.micScenario = Meter()],

                    ["Volume", this.micVolumeControl = InputRangeWithNumber(
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

        this.env.audio.devices.addEventListener("audioinputchanged", (evt) => {
            this.microphones.value = evt.audio && evt.audio.deviceId || "";
        });

        if (canChangeAudioOutput) {
            this.properties.append(
                ["Speakers",
                    this.speakers = Select(
                        onInput(async () => {
                            const deviceId = this.speakers.value;
                            const device = this.spkrLookup.get(deviceId);
                            await this.env.audio.devices.setAudioOutputDevice(device);
                        })
                    )]
            );

            this.env.audio.devices.addEventListener("audiooutputchanged", (evt) => {
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

                this.testSpkrButton = Button("Test",
                    title("Test audio"),
                    className("btn btn-secondary"),
                    styles(
                        marginLeft("0.5em")
                    ),
                    onClick(async () => {
                        buttonSetEnabled(this.testSpkrButton, false, "secondary");
                        await clipLoaded;
                        await this.env.audio.playClipThrough("test-audio");
                        buttonSetEnabled(this.testSpkrButton, true, "secondary");
                    }))],

            this.headphoneWarning = Div(
                className("alert alert-warning"),
                "🎧🎙️ This site has a voice chat feature. Voice chat is best experienced using headphones."
            ),

            ["Volume",
                this.spkrVolumeControl = InputRangeWithNumber(
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
    }

    private async load() {
        await this.env.audio.ready;
        await this.env.audio.devices.ready;

        const mics = await this.env.audio.devices.getAudioInputDevices();
        this.micLookup = makeLookup(mics, m => m.deviceId);

        elementApply(this.microphones,
            Option(value(""), "NONE"),
            ...mics.map((device) =>
                Option(
                    value(device.deviceId),
                    device.label
                )
            )
        );

        if (canChangeAudioOutput) {
            const spkrs = await this.env.audio.devices.getAudioOutputDevices();
            this.spkrLookup = makeLookup(spkrs, device => device.deviceId);

            elementApply(this.speakers,
                ...spkrs.map((device) =>
                    Option(
                        value(device.deviceId),
                        device.label
                    )
                )
            );
        }

        connect(this.env.audio.input, this.activity);
    }

    protected override async onShowing(): Promise<void> {
        if (this.ready === null) {
            this.ready = this.load();
        }
        await this.ready;

        const curMic = await this.env.audio.devices.getAudioInputDevice();
        this.microphones.value = curMic && curMic.deviceId || "";
        this.micVolumeControl.valueAsNumber = this.env.audio.input.gain.value * 100;

        if (canChangeAudioOutput) {
            const curSpker = await this.env.audio.devices.getAudioOutputDevice();
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

    private _showMic = true;
    get showMic(): boolean {
        return this._showMic;
    }

    set showMic(v: boolean) {
        if (v !== this.showMic) {
            this._showMic = v;
            this.properties.setGroupVisible(MIC_GROUP, this.showMic);
        }
    }
}