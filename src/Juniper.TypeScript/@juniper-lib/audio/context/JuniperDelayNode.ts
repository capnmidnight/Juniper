import { IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperDelayNode
    extends JuniperWrappedNode<DelayNode>
    implements DelayNode {

    public readonly delayTime: IAudioParam;

    constructor(context: JuniperAudioContext, options?: DelayOptions) {
        super("delay", context, new DelayNode(context, options));
        this.parent(this.delayTime = new JuniperAudioParam("delay", this.context, this._node.delayTime));
    }
}
