import { autoPlay, controls, id, playsInline, srcObject } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { Audio, ErsatzElement } from "@juniper-lib/dom/tags";
import { IReadyable } from "@juniper-lib/tslib/events/IReadyable";
import { Task } from "@juniper-lib/tslib/events/Task";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { JuniperMediaStreamAudioDestinationNode } from "../context/JuniperMediaStreamAudioDestinationNode";
import { IPoseable } from "../IPoseable";
import type { BaseListener } from "../listeners/BaseListener";
import { WebAudioListenerNew } from "../listeners/WebAudioListenerNew";
import { WebAudioListenerOld } from "../listeners/WebAudioListenerOld";
import { Pose } from "../Pose";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { WebAudioPannerNew } from "../spatializers/WebAudioPannerNew";
import { WebAudioPannerOld } from "../spatializers/WebAudioPannerOld";
import { hasNewAudioListener } from "../util";

export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;

export class WebAudioDestination
    extends BaseNodeCluster<void>
    implements IReadyable, IPoseable, ErsatzElement<HTMLAudioElement> {
    readonly pose = new Pose();
    private readonly volumeControl: JuniperGainNode;
    private readonly destination: JuniperMediaStreamAudioDestinationNode;
    protected readonly listener: BaseListener;
    readonly remoteUserInput: JuniperGainNode
    readonly spatializedInput: JuniperGainNode;
    readonly nonSpatializedInput: JuniperGainNode;
    readonly element: HTMLAudioElement;

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

        const element = Audio(
            id("Audio-Device-Manager"),
            display("none"),
            playsInline(true),
            autoPlay(true),
            controls(true),
            srcObject(destination.stream));

        const ready = new Task();

        onUserGesture(() => element.play());
        element.addEventListener("play", () => ready.resolve());

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
        this.element = element;

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
