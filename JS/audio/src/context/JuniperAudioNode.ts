import { TypedEventMap } from "@juniper-lib/events";
import { BaseNode } from "../BaseNode";
import { IAudioNode, IAudioParam } from "../IAudioNode";
import { InputResolution, JuniperAudioContext, OutputResolution } from "./JuniperAudioContext";


export abstract class JuniperAudioNode<NodeT extends AudioNode = AudioNode, EventsT extends TypedEventMap<string> = TypedEventMap<string>>
    extends BaseNode<EventsT>
    implements IAudioNode {

    constructor(
        type: string,
        context: JuniperAudioContext,
        protected readonly _node: NodeT) {
        super(type, context);
        this.context._init(this._node, this.nodeType);
    }

    protected override onDisposing() {
        this.disconnect();
        this.context._dispose(this._node);
        super.onDisposing();
    }

    protected parent(param: IAudioParam) {
        this.context._parent(this, param);
    }

    get channelCount(): number { return this._node.channelCount; }
    set channelCount(v: number) { this._node.channelCount = v; }
    get channelCountMode(): ChannelCountMode { return this._node.channelCountMode; }
    set channelCountMode(v: ChannelCountMode) { this._node.channelCountMode = v; }
    get channelInterpretation(): ChannelInterpretation { return this._node.channelInterpretation; }
    set channelInterpretation(v: ChannelInterpretation) { this._node.channelInterpretation = v; }
    get numberOfInputs(): number { return this._node.numberOfInputs; }
    get numberOfOutputs(): number { return this._node.numberOfOutputs; }

    _resolveInput(input?: number): InputResolution {
        return {
            destination: this._node,
            input
        };
    }

    _resolveOutput(output?: number): OutputResolution {
        return {
            source: this._node,
            output
        };
    }
}