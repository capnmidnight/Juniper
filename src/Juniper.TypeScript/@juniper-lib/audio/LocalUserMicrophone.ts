import { arrayScan, arraySortByKey } from "@juniper-lib/tslib/collections/arrays";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "./context/JuniperBiquadFilterNode";
import { JuniperDynamicsCompressorNode } from "./context/JuniperDynamicsCompressorNode";
import { JuniperGainNode } from "./context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode } from "./context/JuniperMediaStreamAudioSourceNode";
import { DeviceChangedEvent } from "./DeviceChangedEvent";
import { filterDeviceDuplicates } from "./filterDeviceDuplicates";
import { StreamChangedEvent } from "./StreamChangedEvent";


const PREFERRED_AUDIO_INPUT_ID_KEY = "calla:preferredAudioInputID";

export class LocalUserMicrophone extends BaseNodeCluster<{
    devicechanged: DeviceChangedEvent;
    streamchanged: StreamChangedEvent;
}> {

    private localStreamNode: JuniperMediaStreamAudioSourceNode = null;
    private readonly volume: JuniperGainNode;
    readonly autoGainNode: JuniperGainNode;
    private readonly compressor: JuniperDynamicsCompressorNode;
    private readonly output: JuniperMediaStreamAudioDestinationNode;

    private initTask = Promise.resolve(0);
    private _hasPermission = false;
    private _usingHeadphones = false;
    private _device: MediaDeviceInfo = null;

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

        this.init();
        Object.seal(this);
    }

    get hasPermission(): boolean {
        return this._hasPermission;
    }

    init(): Promise<number> {
        return this.initTask = this.initTask.then((i) => this._initInternal(i));
    }

    private async _initInternal(tryCount: number): Promise<number> {
        if (!this.hasPermission) {
            const devices = tryCount === 0
                ? await navigator.mediaDevices.enumerateDevices()
                : await this.getDevices();
            const anyDevice = arrayScan(devices, dev => dev.kind === "audioinput" && dev.label.length > 0);
            if (isDefined(anyDevice)) {
                this._hasPermission = true;
                this._device = arrayScan(
                    devices,
                    (d) => d.deviceId === this.preferredDeviceID,
                    (d) => d.deviceId === "default",
                    (d) => d.deviceId.length > 0);
            }
        }

        return tryCount + 1;
    }

    get preferredDeviceID(): string {
        return localStorage.getItem(PREFERRED_AUDIO_INPUT_ID_KEY);
    }

    async getDevices(filterDuplicates: boolean = false): Promise<MediaDeviceInfo[]> {
        let devices: MediaDeviceInfo[] = null;
        let testStream: MediaStream = null;
        for (let i = 0; i < 3; ++i) {
            devices = await navigator.mediaDevices.enumerateDevices();

            if (!this.hasPermission) {
                for (const device of devices) {
                    this._hasPermission ||= device.kind === "audioinput"
                        && device.deviceId.length > 0
                        && device.label.length > 0;

                    if (this.hasPermission) {
                        break;
                    }
                }

                if (!this.hasPermission) {
                    try {
                        testStream = await navigator.mediaDevices.getUserMedia({
                            audio: true
                        });
                    }
                    catch (exp) {
                        console.warn(exp);
                    }
                }
            }
        }

        if (testStream) {
            for (const track of testStream.getTracks()) {
                track.stop();
            }
        }

        devices = arraySortByKey(devices || [], (d) => d.label);

        if (filterDuplicates) {
            devices = filterDeviceDuplicates(devices);
        }

        return devices.filter((d) => d.kind === "audioinput");
    }

    get device() {
        return this._device;
    }

    async setDevice(device: MediaDeviceInfo) {
        if (isDefined(device) && device.kind !== "audioinput") {
            throw new Error(`Device is not an audio input device. Was: ${device.kind}. Label: ${device.label}`);
        }

        const curAudioID = this.device && this.device.deviceId || null;
        const nextAudioID = device && device.deviceId || null;
        if (nextAudioID !== curAudioID) {
            this._device = device;
            localStorage.setItem(PREFERRED_AUDIO_INPUT_ID_KEY, nextAudioID);
            this.dispatchEvent(new DeviceChangedEvent(device));
            if (this.inStream) {
                await this.start();
            }
        }
    }

    get inStream() {
        return this.localStreamNode
            && this.localStreamNode.mediaStream
            || null;
    }

    set inStream(mediaStream) {
        if (mediaStream !== this.inStream) {
            const oldStream = this.inStream;

            if (this.localStreamNode) {
                this.remove(this.localStreamNode);

                if (this.inStream
                    && this.inStream.active) {
                    for (const track of this.inStream.getTracks()) {
                        track.stop();
                    }
                }
                this.localStreamNode.dispose();
                this.localStreamNode = null;
            }

            if (mediaStream) {
                this.localStreamNode = new JuniperMediaStreamAudioSourceNode(this.context, {
                    mediaStream
                });
                this.add(this.localStreamNode);
                this.localStreamNode.connect(this.volume);
            }

            this.dispatchEvent(new StreamChangedEvent(this.device, oldStream, this.inStream));
        }
    }

    get outStream() {
        return this.output.stream;
    }

    async start() {
        if (this.device) {
            this.inStream = await navigator.mediaDevices.getUserMedia({
                audio: {
                    deviceId: this.device.deviceId,
                    echoCancellation: !this.usingHeadphones,
                    autoGainControl: true,
                    noiseSuppression: true
                }
            });
        }
    }

    stop() {
        this.inStream = null;
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
            this.start();
        }
    }
}