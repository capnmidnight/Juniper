import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperStereoPannerNode
    extends JuniperAudioNode<StereoPannerNode>
    implements StereoPannerNode {

    public readonly pan: IAudioParam;

    constructor(context: JuniperAudioContext, options?: StereoPannerOptions) {
        super("stereo-panner", context, new StereoPannerNode(context, options));
        this.pan = new JuniperAudioParam("pan", this.context, this._node.pan);
    }
}
