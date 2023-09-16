import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperStereoPannerNode extends JuniperAudioNode {
    constructor(context, options) {
        super("stereo-panner", context, new StereoPannerNode(context, options));
        this.parent(this.pan = new JuniperAudioParam("pan", this.context, this._node.pan));
    }
}
//# sourceMappingURL=JuniperStereoPannerNode.js.map