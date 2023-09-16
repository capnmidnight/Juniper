import { ActivityDetector } from "@juniper-lib/audio/dist/ActivityDetector";
import { canChangeAudioOutput } from "@juniper-lib/audio/dist/SpeakerManager";
import { makeLookup } from "@juniper-lib/collections/dist/makeLookup";
import { Checked, ClassList, Max, Min, Selected, Step, Title_attr, Value } from "@juniper-lib/dom/dist/attrs";
import { display, em, margin, paddingRight } from "@juniper-lib/dom/dist/css";
import { onClick, onInput } from "@juniper-lib/dom/dist/evts";
import { ButtonSecondary, Div, InputCheckbox, Meter, Option, Select, HtmlRender, elementClearChildren, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { AssetFile } from "@juniper-lib/fetcher/dist/Asset";
import { Audio_Mpeg } from "@juniper-lib/mediatypes/dist";
import { stringRandom } from "@juniper-lib/tslib/dist/strings/stringRandom";
import { DialogBox } from "@juniper-lib/widgets/dist/DialogBox";
import { InputRangeWithNumber } from "@juniper-lib/widgets/dist/InputRangeWithNumber";
import { PropertyList, group } from "@juniper-lib/widgets/dist/PropertyList";
const MIC_GROUP = "micFields" + stringRandom(8);
const CAM_GROUP = "camFields" + stringRandom(8);
function makeDeviceSelector(selector, devices, curDevice) {
    elementClearChildren(selector);
    HtmlRender(selector, Option(Value(""), "NONE"), ...devices.map((device) => Option(Selected(curDevice
        && device.deviceId === curDevice.deviceId), Value(device.deviceId), device.label)));
}
export class DeviceDialog extends DialogBox {
    constructor(fetcher, devices, audio, microphones, webcams, DEBUG = false) {
        super("Configure devices");
        this.devices = devices;
        this.audio = audio;
        this.microphones = microphones;
        this.webcams = webcams;
        this.micLookup = null;
        this.camLookup = null;
        this.spkrLookup = null;
        this.spkrVolumeControl = null;
        this.speakers = null;
        this.cancelButton.style.display = "none";
        const clipAsset = new AssetFile("/audio/test-clip.mp3", Audio_Mpeg, !DEBUG);
        const clipLoaded = fetcher.assets(clipAsset)
            .then(() => this.audio.createBasicClip("test-audio", clipAsset, 0.5));
        HtmlRender(this.contentArea, paddingRight("2em"), this.properties = PropertyList.create(group(CAM_GROUP, ["Webcams",
            this.webcamSelector = Select(onInput(async () => {
                const deviceId = this.webcamSelector.value;
                const device = this.camLookup.get(deviceId);
                await this.webcams.setDevice(device);
            }))]), group(MIC_GROUP, ["Microphones",
            this.microphoneSelector = Select(display("inline-block"), onInput(async () => {
                const deviceId = this.microphoneSelector.value;
                const device = this.micLookup.get(deviceId);
                await this.microphones.setDevice(device);
            }))], ["Volume",
            this.micVolumeControl = new InputRangeWithNumber(Min(0), Max(100), Step(1), Value(0), onInput(() => {
                this.microphones.gain.setValueAtTime(this.micVolumeControl.valueAsNumber / 100, 0);
            }))], ["Levels",
            this.micLevels = Meter(display("inline-block"))])));
        if (canChangeAudioOutput) {
            this.properties.append(["Speakers",
                this.speakers = Select(onInput(async () => {
                    const deviceId = this.speakers.value;
                    const device = this.spkrLookup.get(deviceId);
                    await this.audio.speakers.setAudioOutputDevice(device);
                }))]);
            this.audio.speakers.addEventListener("audiooutputchanged", (evt) => {
                this.speakers.value = evt.device && evt.device.deviceId || "";
            });
        }
        this.properties.append(["Volume",
            this.spkrVolumeControl = new InputRangeWithNumber(Min(0), Max(100), Step(1), Value(0), onInput(() => this.audio.destination.volume
                = this.spkrVolumeControl.valueAsNumber / 100))], ["",
            this.testSpkrButton = ButtonSecondary("Test", Title_attr("Test audio"), margin(em(0.5)), onClick(async () => {
                this.testSpkrButton.disabled = true;
                await clipLoaded;
                await this.audio.playClipThrough("test-audio");
                this.testSpkrButton.disabled = false;
            }))], ["Using headphones",
            this.useHeadphones = InputCheckbox(Checked(this.audio.useHeadphones), onInput(() => {
                this.audio.useHeadphones = this.useHeadphones.checked;
                elementSetDisplay(this.headphoneWarning, !this.audio.useHeadphones, "inline-block");
            }))], this.headphoneWarning = Div(ClassList("alert", "alert-warning"), "🎧🎙️ This site has a voice chat feature. Voice chat is best experienced using headphones."));
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
    get showWebcams() {
        return this.properties.getGroupVisible(CAM_GROUP);
    }
    set showWebcams(v) {
        this.properties.setGroupVisible(CAM_GROUP, v);
    }
    get showMicrophones() {
        return this.properties.getGroupVisible(MIC_GROUP);
    }
    set showMicrophones(v) {
        this.properties.setGroupVisible(MIC_GROUP, v);
    }
    async onShowing() {
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
            HtmlRender(this.speakers, ...spkrs.map((device) => Option(Value(device.deviceId), device.label)));
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
//# sourceMappingURL=DeviceDialog.js.map