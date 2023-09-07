import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperGainNode extends JuniperAudioNode {
    constructor(context, options) {
        super("gain", context, new GainNode(context, options));
        this.parent(this.gain = new JuniperAudioParam("gain", this.context, this._node.gain));
    }
}
//# sourceMappingURL=JuniperGainNode.js.map