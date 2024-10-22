import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperAudioWorkletNode
    extends JuniperAudioNode<AudioWorkletNode, {
        processorerror: TypedEvent<"processorerror">
    }>
    implements AudioWorkletNode {

    constructor(context: JuniperAudioContext, name: string, options?: AudioWorkletNodeOptions) {
        super("audio-worklet", context, new AudioWorkletNode(context, name, options));
        this._node.addEventListener("processorerror", () => this.dispatchEvent(new TypedEvent("processorerror")));
    }

    get parameters(): AudioParamMap { return this._node.parameters; }
    get port(): MessagePort { return this._node.port; }

    get onprocessorerror(): (this: AudioWorkletNode, ev: Event) => any { return this._node.onprocessorerror; }
    set onprocessorerror(v: (this: AudioWorkletNode, ev: Event) => any) { this._node.onprocessorerror = v; }
}
