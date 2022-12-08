import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperDelayNode
    extends JuniperAudioNode<DelayNode>
    implements DelayNode {

    public readonly delayTime: IAudioParam;

    constructor(context: JuniperAudioContext, options?: DelayOptions) {
        super("delay", context, new DelayNode(context, options));
        this.delayTime = new JuniperAudioParam("delay", this.context, this._node.delayTime);
    }
}
