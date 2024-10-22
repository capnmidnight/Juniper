import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
import { IAudioNode, IAudioParam } from "./IAudioNode";
import type { InputResolution, JuniperAudioContext, OutputResolution } from "./context/JuniperAudioContext";
export declare abstract class BaseNode<EventMapT extends TypedEventMap<string>> extends TypedEventTarget<EventMapT> implements IAudioNode {
    readonly nodeType: string;
    readonly context: JuniperAudioContext;
    private _name;
    get name(): string;
    set name(v: string);
    constructor(nodeType: string, context: JuniperAudioContext);
    private disposed;
    dispose(): void;
    protected onDisposing(): void;
    isConnected(dest?: IAudioNode | IAudioParam, output?: number, input?: number): boolean;
    resolveOutput(output: number): OutputResolution;
    abstract _resolveOutput(output?: number): OutputResolution;
    resolveInput(input: number): InputResolution;
    abstract _resolveInput(input?: number): InputResolution;
    toggle(destinationParam: IAudioParam, output?: number): void;
    toggle(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    private _toggle;
    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    private _connect;
    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;
    private _disconnect;
    abstract get channelCount(): number;
    abstract set channelCount(v: number);
    abstract get channelCountMode(): ChannelCountMode;
    abstract set channelCountMode(v: ChannelCountMode);
    abstract get channelInterpretation(): ChannelInterpretation;
    abstract set channelInterpretation(v: ChannelInterpretation);
    abstract get numberOfInputs(): number;
    abstract get numberOfOutputs(): number;
}
//# sourceMappingURL=BaseNode.d.ts.map