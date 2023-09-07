import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperOscillatorNode extends JuniperAudioNode {
    constructor(context, options) {
        super("oscillator", context, new OscillatorNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.detune = new JuniperAudioParam("detune", this.context, this._node.detune));
        this.parent(this.frequency = new JuniperAudioParam("frequency", this.context, this._node.frequency));
    }
    get type() { return this._node.type; }
    set type(v) { this._node.type = v; }
    get onended() { return this._node.onended; }
    set onended(v) { this._node.onended = v; }
    setPeriodicWave(periodicWave) { this._node.setPeriodicWave(periodicWave); }
    start(when) { this._node.start(when); }
    stop(when) { this._node.stop(when); }
}
//# sourceMappingURL=JuniperOscillatorNode.js.map