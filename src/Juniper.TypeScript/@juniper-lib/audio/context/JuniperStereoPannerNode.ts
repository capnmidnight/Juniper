import { IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperStereoPannerNode
    extends JuniperWrappedNode<StereoPannerNode>
    implements StereoPannerNode {

    public readonly pan: IAudioParam;

    constructor(context: JuniperAudioContext, options?: StereoPannerOptions) {
        super("stereo-panner", context, new StereoPannerNode(context, options));
        this.parent(this.pan = new JuniperAudioParam("pan", this.context, this._node.pan));
    }
}
