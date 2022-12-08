import { IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperBiquadFilterNode
    extends JuniperWrappedNode<BiquadFilterNode>
    implements BiquadFilterNode {

    public readonly Q: IAudioParam;
    public readonly detune: IAudioParam;
    public readonly frequency: IAudioParam;
    public readonly gain: IAudioParam;

    constructor(context: JuniperAudioContext, options?: BiquadFilterOptions) {
        super("biquad-filter", context, new BiquadFilterNode(context, options));
        this.Q = new JuniperAudioParam("Q", this.context, this._node.Q);
        this.detune = new JuniperAudioParam("detune", this.context, this._node.detune);
        this.frequency = new JuniperAudioParam("frequency", this.context, this._node.frequency);
        this.gain = new JuniperAudioParam("gain", this.context, this._node.gain);
    }

    get type(): BiquadFilterType { return this._node.type; }
    set type(v: BiquadFilterType) { this._node.type = v; }

    getFrequencyResponse(frequencyHz: Float32Array, magResponse: Float32Array, phaseResponse: Float32Array): void {
        this._node.getFrequencyResponse(frequencyHz, magResponse, phaseResponse);
    }
}
