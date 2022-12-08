import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperGainNode
    extends JuniperAudioNode<GainNode>
    implements GainNode {

    public readonly gain: IAudioParam;

    constructor(context: JuniperAudioContext, options?: GainOptions) {
        super("gain", context, new GainNode(context, options));
        this.gain = new JuniperAudioParam("gain", this.context, this._node.gain);
    }
}