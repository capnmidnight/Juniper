import { TypedEventMap } from "@juniper-lib/events/TypedEventTarget";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";

export class JuniperAnalyserNode<EventsT extends TypedEventMap<string> = TypedEventMap<string>>
    extends JuniperAudioNode<AnalyserNode, EventsT>
    implements AnalyserNode {

    constructor(context: JuniperAudioContext, options?: AnalyserOptions) {
        super("analyser", context, new AnalyserNode(context, options));
    }

    get fftSize(): number { return this._node.fftSize; }
    set fftSize(v: number) { this._node.fftSize = v; }
    get frequencyBinCount(): number { return this._node.frequencyBinCount; }
    get maxDecibels(): number { return this._node.maxDecibels; }
    set maxDecibels(v: number) { this._node.maxDecibels = v; }
    get minDecibels(): number { return this._node.minDecibels; }
    set minDecibels(v: number) { this._node.minDecibels = v; }
    get smoothingTimeConstant(): number { return this._node.smoothingTimeConstant; }
    set smoothingTimeConstant(v: number) { this._node.smoothingTimeConstant = v; }
    getByteFrequencyData(array: Uint8Array): void { this._node.getByteFrequencyData(array); }
    getByteTimeDomainData(array: Uint8Array): void { this._node.getByteTimeDomainData(array); }
    getFloatFrequencyData(array: Float32Array): void { this._node.getFloatFrequencyData(array); }
    getFloatTimeDomainData(array: Float32Array): void { this._node.getFloatTimeDomainData(array); }
}

