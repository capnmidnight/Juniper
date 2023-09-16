import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperBiquadFilterNode } from "./context/JuniperBiquadFilterNode";
import { JuniperDynamicsCompressorNode } from "./context/JuniperDynamicsCompressorNode";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode } from "./context/JuniperMediaStreamAudioSourceNode";
import { DeviceSettingsChangedEvent } from "./DeviceManager";
import { StreamChangedEvent } from "./StreamChangedEvent";
import { dispose } from "@juniper-lib/tslib/dist/using";
const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";
export class LocalUserMicrophone extends BaseNodeCluster {
    constructor(context) {
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
        this.localStreamNode = null;
        this._hasPermission = false;
        this._usingHeadphones = false;
        this._device = null;
        this._enabled = false;
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
    get mediaType() {
        return "audio";
    }
    get deviceKind() {
        return `${this.mediaType}input`;
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        if (v !== this.enabled) {
            this._enabled = v;
            this.onChange();
        }
    }
    get hasPermission() {
        return this._hasPermission;
    }
    get preferredDeviceID() {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }
    get device() {
        return this._device;
    }
    async setDevice(device) {
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
    get muted() {
        return this.compressor.isConnected(this.output);
    }
    set muted(v) {
        if (v !== this.muted) {
            if (v) {
                this.compressor.connect(this.output);
            }
            else {
                this.compressor.disconnect(this.output);
            }
        }
    }
    get usingHeadphones() {
        return this._usingHeadphones;
    }
    set usingHeadphones(v) {
        if (v !== this.usingHeadphones) {
            this._usingHeadphones = v;
            this.onChange();
        }
    }
    async onChange() {
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
//# sourceMappingURL=LocalUserMicrophone.js.map