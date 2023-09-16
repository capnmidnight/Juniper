import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperConstantSourceNode extends JuniperAudioNode {
    constructor(context, options) {
        super("constant-source", context, new ConstantSourceNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.offset = new JuniperAudioParam("offset", this.context, this._node.offset));
    }
    get onended() { return this._node.onended; }
    set onended(v) { this._node.onended = v; }
    start(when) { this._node.start(when); }
    stop(when) { this._node.stop(when); }
}
//# sourceMappingURL=JuniperConstantSourceNode.js.map