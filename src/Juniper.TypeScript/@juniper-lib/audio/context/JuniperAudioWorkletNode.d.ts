import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperAudioWorkletNode extends JuniperAudioNode<AudioWorkletNode, {
    processorerror: TypedEvent<"processorerror">;
}> implements AudioWorkletNode {
    constructor(context: JuniperAudioContext, name: string, options?: AudioWorkletNodeOptions);
    get parameters(): AudioParamMap;
    get port(): MessagePort;
    get onprocessorerror(): (this: AudioWorkletNode, ev: Event) => any;
    set onprocessorerror(v: (this: AudioWorkletNode, ev: Event) => any);
}
//# sourceMappingURL=JuniperAudioWorkletNode.d.ts.map