import { IAudioNode } from "./IAudioNode";
import { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperBaseNode } from "./JuniperBaseNode";


export abstract class JuniperWrappedNode<NodeT extends AudioNode = AudioNode, EventsT = void>
    extends JuniperBaseNode<EventsT>
    implements IAudioNode {

    constructor(
        type: string,
        context: JuniperAudioContext,
        protected readonly _node: NodeT) {
        super(type, context);
    }

    protected override onDisposing() {
        this.disconnect();
        super.onDisposing();
    }

    get channelCount(): number { return this._node.channelCount; }
    set channelCount(v: number) { this._node.channelCount = v; }
    get channelCountMode(): ChannelCountMode { return this._node.channelCountMode; }
    set channelCountMode(v: ChannelCountMode) { this._node.channelCountMode = v; }
    get channelInterpretation(): ChannelInterpretation { return this._node.channelInterpretation; }
    set channelInterpretation(v: ChannelInterpretation) { this._node.channelInterpretation = v; }
    get numberOfInputs(): number { return this._node.numberOfInputs; }
    get numberOfOutputs(): number { return this._node.numberOfOutputs; }

    _resolveInput(input?: number): { destination: AudioNode | AudioParam; input?: number; } {
        return {
            destination: this._node,
            input
        };
    }

    protected _resolveOutput(output?: number): { source: AudioNode; output?: number } {
        return {
            source: this._node,
            output
        }
    }
}