import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperBiquadFilterNode extends JuniperAudioNode<BiquadFilterNode> implements BiquadFilterNode {
    readonly Q: IAudioParam;
    readonly detune: IAudioParam;
    readonly frequency: IAudioParam;
    readonly gain: IAudioParam;
    constructor(context: JuniperAudioContext, options?: BiquadFilterOptions);
    get type(): BiquadFilterType;
    set type(v: BiquadFilterType);
    getFrequencyResponse(frequencyHz: Float32Array, magResponse: Float32Array, phaseResponse: Float32Array): void;
}
//# sourceMappingURL=JuniperBiquadFilterNode.d.ts.map