import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "./context/JuniperBiquadFilterNode";
import { JuniperDynamicsCompressorNode } from "./context/JuniperDynamicsCompressorNode";
import { JuniperGainNode } from "./context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode } from "./context/JuniperMediaStreamAudioSourceNode";
import { DeviceSettingsChangedEvent, IDeviceSource } from "./DeviceManager";
import { StreamChangedEvent } from "./StreamChangedEvent";
import { dispose } from "@juniper-lib/tslib/dist/using";


const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";


export class LocalUserMicrophone extends BaseNodeCluster<{
    devicesettingschanged: DeviceSettingsChangedEvent;
    streamchanged: StreamChangedEvent;
}> implements IDeviceSource {

    private localStreamNode: JuniperMediaStreamAudioSourceNode = null;
    private readonly volume: JuniperGainNode;
    readonly autoGainNode: JuniperGainNode;
    private readonly compressor: JuniperDynamicsCompressorNode;
    private readonly output: JuniperMediaStreamAudioDestinationNode;

    private _hasPermission = false;
    private _usingHeadphones = false;
    private _device: MediaDeviceInfo = null;
    private _enabled = false;

    constructor(context: JuniperAudioContext) {
        const volume = context.createGain();
        volume.name = "local-mic-user-gain";

        const autoGainNode = context.createGain();
        autoGainNode.name = "local-mic-auto-gain";

        const filter = new JuniperBiquadFilterNode(context, {
            type: "bandpass",
            frequency: 1500,
            Q: 0.25,
        });
        filter.name = "local-mic-filter";

        const compressor = new JuniperDynamicsCompressorNode(context, {
            threshold: -15,
            knee: 40,
            ratio: 17
        });
        compressor.name = "local-mic-compressor";

        const localOutput = new JuniperMediaStreamAudioDestinationNode(context);
        localOutput.name = "local-mic-destination";

        super("local-user-microphone", context, [], [compressor], [
            volume,
            filter,
            localOutput
        ]);

        this.volume = volume;
        this.autoGainNode = autoGainNode;
        this.compressor = compressor;
        this.output = localOutput;

        volume
            .connect(autoGainNode)
            .connect(filter)
            .connect(compressor)
            .connect(localOutput);

        Object.seal(this);
    }

    get mediaType(): "audio" | "video" {
        return "audio";
    }

    get deviceKind(): MediaDeviceKind {
        return `${this.mediaType}input`;
    }

    get enabled(): boolean {
        return this._enabled;
    }

    set enabled(v: boolean) {
        if (v !== this.enabled) {
            this._enabled = v;
            this.onChange();
        }
    }

    get hasPermission(): boolean {
        return this._hasPermission;
    }

    get preferredDeviceID(): string {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }

    get device() {
        return this._device;
    }

    async setDevice(device: MediaDeviceInfo) {
        if (isDefined(device) && device.kind !== this.deviceKind) {
            throw new Error(`Device is not an audio input device. Was: ${device.kind}. Label: ${device.label}`);
        }

        const curAudioID = this.device && this.device.deviceId || null;
        const nextAudioID = device && device.deviceId || null;
        if (nextAudioID !== curAudioID) {
            this._device = device;
            localStorage.setItem(PREFERRED_AUDIO_INPUT_ID_KEY, nextAudioID);
            this.onChange();
        }
    }

    get inStream() {
        return this.localStreamNode
            && this.localStreamNode.mediaStream
            || null;
    }

    set inStream(mediaStream) {
        if (mediaStream !== this.inStream) {
            if (this.localStreamNode) {
                this.remove(this.localStreamNode);
                dispose(this.localStreamNode);
                this.localStreamNode = null;
            }

            if (mediaStream) {
                this.localStreamNode = new JuniperMediaStreamAudioSourceNode(this.context, {
                    mediaStream
                });
                this.add(this.localStreamNode);
                this.localStreamNode.connect(this.volume);
            }
        }
    }

    get outStream() {
        return this.output.stream;
    }

    get gain() {
        return this.volume.gain;
    }

    get muted(): boolean {
        return this.compressor.isConnected(this.output);
    }

    set muted(v: boolean) {
        if (v !== this.muted) {
            if (v) {
                this.compressor.connect(this.output);
            }
            else {
                this.compressor.disconnect(this.output);
            }
        }
    }

    get usingHeadphones(): boolean {
        return this._usingHeadphones;
    }

    set usingHeadphones(v: boolean) {
        if (v !== this.usingHeadphones) {
            this._usingHeadphones = v;
            this.onChange();
        }
    }

    private async onChange() {
        this.dispatchEvent(new DeviceSettingsChangedEvent());
        const oldStream = this.inStream;
        if (this.device && this.enabled) {
            this.inStream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    deviceId: this.device.deviceId,
                    echoCancellation: !this.usingHeadphones,
                    autoGainControl: true,
                    noiseSuppression: true
                }
            });
        }
        else {
            this.inStream = null;
        }
        if (this.inStream !== oldStream) {
            this.dispatchEvent(new StreamChangedEvent(oldStream, this.outStream));
        }
    }
}