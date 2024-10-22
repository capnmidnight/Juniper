import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperDynamicsCompressorNode extends JuniperAudioNode {
    constructor(context, options) {
        super("dynamics-compressor", context, new DynamicsCompressorNode(context, options));
        this.parent(this.attack = new JuniperAudioParam("attack", this.context, this._node.attack));
        this.parent(this.knee = new JuniperAudioParam("knee", this.context, this._node.knee));
        this.parent(this.ratio = new JuniperAudioParam("ratio", this.context, this._node.ratio));
        this.parent(this.release = new JuniperAudioParam("release", this.context, this._node.release));
        this.parent(this.threshold = new JuniperAudioParam("threshold", this.context, this._node.threshold));
    }
    get reduction() { return this._node.reduction; }
}
//# sourceMappingURL=JuniperDynamicsCompressorNode.js.map