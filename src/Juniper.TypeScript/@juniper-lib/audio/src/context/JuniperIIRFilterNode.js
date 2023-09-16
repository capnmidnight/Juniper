import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperIIRFilterNode extends JuniperAudioNode {
    constructor(context, options) {
        super("iir-filter", context, new IIRFilterNode(context, options));
    }
    getFrequencyResponse(frequencyHz, magResponse, phaseResponse) {
        this._node.getFrequencyResponse(frequencyHz, magResponse, phaseResponse);
    }
}
//# sourceMappingURL=JuniperIIRFilterNode.js.map