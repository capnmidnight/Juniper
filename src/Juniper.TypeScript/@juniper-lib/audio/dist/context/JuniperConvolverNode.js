import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperConvolverNode extends JuniperAudioNode {
    constructor(context, options) {
        super("convolver", context, new ConvolverNode(context, options));
    }
    get buffer() { return this._node.buffer; }
    set buffer(v) { this._node.buffer = v; }
    get normalize() { return this._node.normalize; }
    set normalize(v) { this._node.normalize = v; }
}
//# sourceMappingURL=JuniperConvolverNode.js.map