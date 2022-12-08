import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperConvolverNode
    extends JuniperAudioNode<ConvolverNode>
    implements ConvolverNode {

    constructor(context: JuniperAudioContext, options?: ConvolverOptions) {
        super("convolver", context, new ConvolverNode(context, options));
    }

    get buffer(): AudioBuffer { return this._node.buffer; }
    set buffer(v: AudioBuffer) { this._node.buffer = v; }
    get normalize(): boolean { return this._node.normalize; }
    set normalize(v: boolean) { this._node.normalize = v; }
}
