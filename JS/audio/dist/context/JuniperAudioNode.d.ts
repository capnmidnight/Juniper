import { TypedEventMap } from "@juniper-lib/events";
import { BaseNode } from "../BaseNode";
import { IAudioNode, IAudioParam } from "../IAudioNode";
import { InputResolution, JuniperAudioContext, OutputResolution } from "./JuniperAudioContext";
export declare abstract class JuniperAudioNode<NodeT extends AudioNode = AudioNode, EventsT extends TypedEventMap<string> = TypedEventMap<string>> extends BaseNode<EventsT> implements IAudioNode {
    protected readonly _node: NodeT;
    constructor(type: string, context: JuniperAudioContext, _node: NodeT);
    protected onDisposing(): void;
    protected parent(param: IAudioParam): void;
    get channelCount(): number;
    set channelCount(v: number);
    get channelCountMode(): ChannelCountMode;
    set channelCountMode(v: ChannelCountMode);
    get channelInterpretation(): ChannelInterpretation;
    set channelInterpretation(v: ChannelInterpretation);
    get numberOfInputs(): number;
    get numberOfOutputs(): number;
    _resolveInput(input?: number): InputResolution;
    _resolveOutput(output?: number): OutputResolution;
}
//# sourceMappingURL=JuniperAudioNode.d.ts.map