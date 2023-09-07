import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperConstantSourceNode extends JuniperAudioNode<ConstantSourceNode, {
    ended: TypedEvent<"ended">;
}> implements ConstantSourceNode {
    readonly offset: IAudioParam;
    constructor(context: JuniperAudioContext, options?: ConstantSourceOptions);
    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any;
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any);
    start(when?: number): void;
    stop(when?: number): void;
}
//# sourceMappingURL=JuniperConstantSourceNode.d.ts.map