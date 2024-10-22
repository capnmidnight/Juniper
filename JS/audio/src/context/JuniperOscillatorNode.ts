import { TypedEvent } from "@juniper-lib/events";
import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperOscillatorNode
    extends JuniperAudioNode<OscillatorNode, {
        ended: TypedEvent<"ended">;
    }>
    implements OscillatorNode {

    public readonly detune: IAudioParam;
    public readonly frequency: IAudioParam;

    constructor(context: JuniperAudioContext, options?: OscillatorOptions) {
        super("oscillator", context, new OscillatorNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.detune = new JuniperAudioParam("detune", this.context, this._node.detune));
        this.parent(this.frequency = new JuniperAudioParam("frequency", this.context, this._node.frequency));
    }

    get type(): OscillatorType { return this._node.type; }
    set type(v: OscillatorType) { this._node.type = v; }

    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any { return this._node.onended; }
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any) { this._node.onended = v; }

    setPeriodicWave(periodicWave: PeriodicWave): void { this._node.setPeriodicWave(periodicWave); }
    start(when?: number): void { this._node.start(when); }
    stop(when?: number): void { this._node.stop(when); }
}
