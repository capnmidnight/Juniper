import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperAnalyserNode extends JuniperAudioNode {
    constructor(context, options) {
        super("analyser", context, new AnalyserNode(context, options));
    }
    get fftSize() { return this._node.fftSize; }
    set fftSize(v) { this._node.fftSize = v; }
    get frequencyBinCount() { return this._node.frequencyBinCount; }
    get maxDecibels() { return this._node.maxDecibels; }
    set maxDecibels(v) { this._node.maxDecibels = v; }
    get minDecibels() { return this._node.minDecibels; }
    set minDecibels(v) { this._node.minDecibels = v; }
    get smoothingTimeConstant() { return this._node.smoothingTimeConstant; }
    set smoothingTimeConstant(v) { this._node.smoothingTimeConstant = v; }
    getByteFrequencyData(array) { this._node.getByteFrequencyData(array); }
    getByteTimeDomainData(array) { this._node.getByteTimeDomainData(array); }
    getFloatFrequencyData(array) { this._node.getFloatFrequencyData(array); }
    getFloatTimeDomainData(array) { this._node.getFloatTimeDomainData(array); }
}
//# sourceMappingURL=JuniperAnalyserNode.js.map