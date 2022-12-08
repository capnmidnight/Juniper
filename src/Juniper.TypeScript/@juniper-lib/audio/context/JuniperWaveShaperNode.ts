import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperWaveShaperNode
    extends JuniperWrappedNode<WaveShaperNode>
    implements WaveShaperNode {

    constructor(context: JuniperAudioContext, options?: WaveShaperOptions) {
        super("wave-shaper", context, new WaveShaperNode(context, options));
    }

    get curve(): Float32Array { return this._node.curve; }
    set curve(v: Float32Array) { this._node.curve = v; }
    get oversample(): OverSampleType { return this._node.oversample; }
    set oversample(v: OverSampleType) { this._node.oversample = v; }
}
