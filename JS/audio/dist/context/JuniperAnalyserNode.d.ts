import { TypedEventMap } from "@juniper-lib/events";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperAnalyserNode<EventsT extends TypedEventMap<string> = TypedEventMap<string>> extends JuniperAudioNode<AnalyserNode, EventsT> implements AnalyserNode {
    constructor(context: JuniperAudioContext, options?: AnalyserOptions);
    get fftSize(): number;
    set fftSize(v: number);
    get frequencyBinCount(): number;
    get maxDecibels(): number;
    set maxDecibels(v: number);
    get minDecibels(): number;
    set minDecibels(v: number);
    get smoothingTimeConstant(): number;
    set smoothingTimeConstant(v: number);
    getByteFrequencyData(array: Uint8Array): void;
    getByteTimeDomainData(array: Uint8Array): void;
    getFloatFrequencyData(array: Float32Array): void;
    getFloatTimeDomainData(array: Float32Array): void;
}
//# sourceMappingURL=JuniperAnalyserNode.d.ts.map