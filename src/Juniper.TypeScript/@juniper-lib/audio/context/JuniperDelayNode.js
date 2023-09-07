import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperDelayNode extends JuniperAudioNode {
    constructor(context, options) {
        super("delay", context, new DelayNode(context, options));
        this.parent(this.delayTime = new JuniperAudioParam("delay", this.context, this._node.delayTime));
    }
}
//# sourceMappingURL=JuniperDelayNode.js.map