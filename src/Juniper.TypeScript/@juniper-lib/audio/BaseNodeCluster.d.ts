import { InputResolution, JuniperAudioContext, OutputResolution } from "./context/JuniperAudioContext";
import { BaseNode } from "./BaseNode";
import { IAudioNode, IAudioParam } from "./IAudioNode";
import { TypedEventMap } from "@juniper-lib/events/TypedEventTarget";
export declare abstract class BaseNodeCluster<EventsT extends TypedEventMap<string> = TypedEventMap<string>> extends BaseNode<EventsT> implements IAudioNode {
    private readonly inputs;
    private readonly outputs;
    private readonly allNodes;
    private get exemplar();
    constructor(type: string, context: JuniperAudioContext, inputs?: ReadonlyArray<IAudioNode | IAudioParam>, endpoints?: ReadonlyArray<IAudioNode | IAudioParam>, extras?: ReadonlyArray<IAudioNode>);
    protected add(node: IAudioNode): void;
    protected remove(node: IAudioNode): void;
    protected onDisposing(): void;
    get channelCount(): number;
    set channelCount(v: number);
    get channelCountMode(): ChannelCountMode;
    set channelCountMode(v: ChannelCountMode);
    get channelInterpretation(): ChannelInterpretation;
    set channelInterpretation(v: ChannelInterpretation);
    get numberOfInputs(): number;
    get numberOfOutputs(): number;
    private static resolve;
    _resolveInput(input?: number): InputResolution;
    _resolveOutput(output?: number): OutputResolution;
}
//# sourceMappingURL=BaseNodeCluster.d.ts.map