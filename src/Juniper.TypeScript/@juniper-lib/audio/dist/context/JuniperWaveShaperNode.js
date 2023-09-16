import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperWaveShaperNode extends JuniperAudioNode {
    constructor(context, options) {
        super("wave-shaper", context, new WaveShaperNode(context, options));
    }
    get curve() { return this._node.curve; }
    set curve(v) { this._node.curve = v; }
    get oversample() { return this._node.oversample; }
    set oversample(v) { this._node.oversample = v; }
}
//# sourceMappingURL=JuniperWaveShaperNode.js.map