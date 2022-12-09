import { IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperGainNode
    extends JuniperWrappedNode<GainNode>
    implements GainNode {

    public readonly gain: IAudioParam;

    constructor(context: JuniperAudioContext, options?: GainOptions) {
        super("gain", context, new GainNode(context, options));
        this.parent(this.gain = new JuniperAudioParam("gain", this.context, this._node.gain));
    }
}