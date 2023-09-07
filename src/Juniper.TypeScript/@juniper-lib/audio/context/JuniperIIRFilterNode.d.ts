import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperIIRFilterNode extends JuniperAudioNode<IIRFilterNode> implements IIRFilterNode {
    constructor(context: JuniperAudioContext, options: IIRFilterOptions);
    getFrequencyResponse(frequencyHz: Float32Array, magResponse: Float32Array, phaseResponse: Float32Array): void;
}
//# sourceMappingURL=JuniperIIRFilterNode.d.ts.map