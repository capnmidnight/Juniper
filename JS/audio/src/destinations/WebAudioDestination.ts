import { Audio, AutoPlay, Controls, display, ID, onUserGesture, SrcObject, OnPlay } from "@juniper-lib/dom";
import { IReadyable, Task } from "@juniper-lib/events";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { IPoseable } from "../IPoseable";
import { Pose } from "../Pose";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "../context/JuniperMediaStreamAudioDestinationNode";
import type { BaseListener } from "../listeners/BaseListener";
import { WebAudioListenerNew } from "../listeners/WebAudioListenerNew";
import { WebAudioListenerOld } from "../listeners/WebAudioListenerOld";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { WebAudioPannerNew } from "../spatializers/WebAudioPannerNew";
import { WebAudioPannerOld } from "../spatializers/WebAudioPannerOld";
import { hasNewAudioListener } from "../util";

export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;

export class WebAudioDestination
    extends BaseNodeCluster
    implements IReadyable, IPoseable {
    readonly pose = new Pose();
    private readonly volumeControl: JuniperGainNode;
    private readonly destination: JuniperMediaStreamAudioDestinationNode;
    protected readonly listener: BaseListener;
    readonly remoteUserInput: JuniperGainNode;
    readonly spatializedInput: JuniperGainNode;
    readonly nonSpatializedInput: JuniperGainNode;
    readonly audioElement: HTMLAudioElement;

    private readonly _ready: Task;
    get ready(): Promise<void> { return this._ready; }
    get isReady() { return this._ready.finished && this._ready.resolved; }

    constructor(context: JuniperAudioContext) {
        const listener = hasNewAudioListener
            ? new WebAudioListenerNew(context)
            : new WebAudioListenerOld(context);

        const remoteUserInput = new JuniperGainNode(context);
        remoteUserInput.name = "remote-user-input";

        const spatializedInput = new JuniperGainNode(context);
        spatializedInput.name = "spatialized-input";

        const nonSpatializedInput = new JuniperGainNode(context);
        nonSpatializedInput.name = "non-spatialized-input";

        const destination = new JuniperMediaStreamAudioDestinationNode(context);

        const ready = new Task();
        const element = Audio(
            ID("Audio-Device-Manager"),
            display("none"),
            AutoPlay(true),
            Controls(true),
            SrcObject(destination.stream),
            OnPlay(() => ready.resolve())
        );

        onUserGesture(() => element.play());

        super("web-audio-destination", context,
            [nonSpatializedInput, spatializedInput, remoteUserInput],
            [],
            [destination]);

        this._ready = ready;
        this.listener = listener;
        this.remoteUserInput = remoteUserInput;
        this.spatializedInput = spatializedInput;
        this.nonSpatializedInput = nonSpatializedInput;
        this.volumeControl = nonSpatializedInput;
        this.destination = destination;
        this.audioElement = element;

        remoteUserInput
            .connect(spatializedInput)
            .connect(this.volumeControl)
            .connect(destination);
    }

    createSpatializer(isRemoteStream: boolean): BaseSpatializer {
        const destination = isRemoteStream
            ? this.remoteUserInput
            : this.spatializedInput;

        const spatializer = hasNewAudioListener
            ? new WebAudioPannerNew(this.context)
            : new WebAudioPannerOld(this.context);

        spatializer.connect(destination);
        return spatializer;
    }

    setPosition(px: number, py: number, pz: number): void {
        this.pose.setPosition(px, py, pz);
        this.listener.readPose(this.pose);
    }

    setOrientation(qx: number, qy: number, qz: number, qw: number): void {
        this.pose.setOrientation(qx, qy, qz, qw);
        this.listener.readPose(this.pose);
    }

    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void {
        this.pose.set(px, py, pz, qx, qy, qz, qw);
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
