import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperAudioNode } from "../context/JuniperAudioNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "../context/JuniperMediaStreamAudioDestinationNode";
import { IPoseable } from "../IPoseable";
import type { BaseListener } from "../listeners/BaseListener";
import { Pose } from "../Pose";

export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;

export class WebAudioDestination extends JuniperAudioNode<void> implements IPoseable {
    readonly pose = new Pose();
    readonly remoteUserInputIndex: number
    readonly spatializedInputIndex: number;
    readonly nonSpatializedInputIndex: number;
    private readonly volumeControl: JuniperGainNode;
    private readonly destination: JuniperMediaStreamAudioDestinationNode;

    constructor(context: JuniperAudioContext, protected readonly listener: BaseListener) {
        const remoteUserInput = new JuniperGainNode(context);
        remoteUserInput.name = "remote-user-input";

        const spatializedInput = new JuniperGainNode(context);
        spatializedInput.name = "spatialized-input";

        const destination = new JuniperMediaStreamAudioDestinationNode(context);

        const volumeControl = new JuniperGainNode(context);

        super("web-audio-destination", context,
            [remoteUserInput, spatializedInput, volumeControl],
            null,
            [destination, volumeControl]);

        this.remoteUserInputIndex = 0;
        this.spatializedInputIndex = 1;
        this.nonSpatializedInputIndex = 2;

        this.name = "final";

        this.volumeControl = volumeControl;
        this.destination = destination;

        remoteUserInput.disconnect(this.volumeControl);

        remoteUserInput
            .connect(spatializedInput)
            .connect(this.volumeControl)
            .connect(destination);
    }

    setPosition(px: number, py: number, pz: number): void {
        this.pose.setPosition(px, py, pz);
        this.listener.readPose(this.pose);
    }

    setOrientation(fx: number, fy: number, fz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        this.pose.setOrientation(fx, fy, fz, ux, uy, uz);
        this.listener.readPose(this.pose);
    }

    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        this.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
        this.listener.readPose(this.pose);
    }

    get stream() {
        return this.destination.stream;
    }

    get volume(): number {
        return this.volumeControl.gain.value;
    }

    set volume(v: number) {
        this.volumeControl.gain.value = v;
    }
}
