import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperConstantSourceNode
    extends JuniperAudioNode<ConstantSourceNode, {
        ended: TypedEvent<"ended">;
    }>
    implements ConstantSourceNode {

    public readonly offset: IAudioParam;

    constructor(context: JuniperAudioContext, options?: ConstantSourceOptions) {
        super("constant-source", context, new ConstantSourceNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.offset = new JuniperAudioParam("offset", this.context, this._node.offset));
    }

    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any { return this._node.onended; }
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any) { this._node.onended = v; }

    start(when?: number): void { this._node.start(when); }
    stop(when?: number): void { this._node.stop(when); }
}