import { IReadyable } from "@juniper-lib/events";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { IPoseable } from "../IPoseable";
import { Pose } from "../Pose";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import type { BaseListener } from "../listeners/BaseListener";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
export type DestinationNode = AudioDestinationNode | MediaStreamAudioDestinationNode;
export declare class WebAudioDestination extends BaseNodeCluster implements IReadyable, IPoseable {
    readonly pose: Pose;
    private readonly volumeControl;
    private readonly destination;
    protected readonly listener: BaseListener;
    readonly remoteUserInput: JuniperGainNode;
    readonly spatializedInput: JuniperGainNode;
    readonly nonSpatializedInput: JuniperGainNode;
    readonly audioElement: HTMLAudioElement;
    private readonly _ready;
    get ready(): Promise<void>;
    get isReady(): boolean;
    constructor(context: JuniperAudioContext);
    createSpatializer(isRemoteStream: boolean): BaseSpatializer;
    setPosition(px: number, py: number, pz: number): void;
    setOrientation(qx: number, qy: number, qz: number, qw: number): void;
    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void;
    get stream(): MediaStream;
    get volume(): number;
    set volume(v: number);
}
//# sourceMappingURL=WebAudioDestination.d.ts.map