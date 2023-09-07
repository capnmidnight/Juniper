import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperWaveShaperNode extends JuniperAudioNode<WaveShaperNode> implements WaveShaperNode {
    constructor(context: JuniperAudioContext, options?: WaveShaperOptions);
    get curve(): Float32Array;
    set curve(v: Float32Array);
    get oversample(): OverSampleType;
    set oversample(v: OverSampleType);
}
//# sourceMappingURL=JuniperWaveShaperNode.d.ts.map