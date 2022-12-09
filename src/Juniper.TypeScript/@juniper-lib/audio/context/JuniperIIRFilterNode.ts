import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperIIRFilterNode
    extends JuniperWrappedNode<IIRFilterNode>
    implements IIRFilterNode {

    constructor(context: JuniperAudioContext, options: IIRFilterOptions) {
        super("iir-filter", context, new IIRFilterNode(context, options));
    }

    getFrequencyResponse(frequencyHz: Float32Array, magResponse: Float32Array, phaseResponse: Float32Array): void {
        this._node.getFrequencyResponse(frequencyHz, magResponse, phaseResponse);
    }
}
