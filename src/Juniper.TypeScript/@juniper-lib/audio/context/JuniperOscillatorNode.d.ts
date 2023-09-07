import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperOscillatorNode extends JuniperAudioNode<OscillatorNode, {
    ended: TypedEvent<"ended">;
}> implements OscillatorNode {
    readonly detune: IAudioParam;
    readonly frequency: IAudioParam;
    constructor(context: JuniperAudioContext, options?: OscillatorOptions);
    get type(): OscillatorType;
    set type(v: OscillatorType);
    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any;
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any);
    setPeriodicWave(periodicWave: PeriodicWave): void;
    start(when?: number): void;
    stop(when?: number): void;
}
//# sourceMappingURL=JuniperOscillatorNode.d.ts.map