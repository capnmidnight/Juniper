import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperAudioWorkletNode extends JuniperAudioNode {
    constructor(context, name, options) {
        super("audio-worklet", context, new AudioWorkletNode(context, name, options));
        this._node.addEventListener("processorerror", () => this.dispatchEvent(new TypedEvent("processorerror")));
    }
    get parameters() { return this._node.parameters; }
    get port() { return this._node.port; }
    get onprocessorerror() { return this._node.onprocessorerror; }
    set onprocessorerror(v) { this._node.onprocessorerror = v; }
}
//# sourceMappingURL=JuniperAudioWorkletNode.js.map