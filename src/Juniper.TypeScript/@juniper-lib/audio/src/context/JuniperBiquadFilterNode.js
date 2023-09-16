import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperBiquadFilterNode extends JuniperAudioNode {
    constructor(context, options) {
        super("biquad-filter", context, new BiquadFilterNode(context, options));
        this.parent(this.Q = new JuniperAudioParam("Q", this.context, this._node.Q));
        this.parent(this.detune = new JuniperAudioParam("detune", this.context, this._node.detune));
        this.parent(this.frequency = new JuniperAudioParam("frequency", this.context, this._node.frequency));
        this.parent(this.gain = new JuniperAudioParam("gain", this.context, this._node.gain));
    }
    get type() { return this._node.type; }
    set type(v) { this._node.type = v; }
    getFrequencyResponse(frequencyHz, magResponse, phaseResponse) {
        this._node.getFrequencyResponse(frequencyHz, magResponse, phaseResponse);
    }
}
//# sourceMappingURL=JuniperBiquadFilterNode.js.map