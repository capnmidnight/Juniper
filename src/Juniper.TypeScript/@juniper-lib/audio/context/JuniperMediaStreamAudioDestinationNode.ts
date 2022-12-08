import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperMediaStreamAudioDestinationNode
    extends JuniperWrappedNode<MediaStreamAudioDestinationNode>
    implements MediaStreamAudioDestinationNode {

    constructor(context: JuniperAudioContext, options?: AudioNodeOptions) {
        super("media-stream-audio-destination", context, new MediaStreamAudioDestinationNode(context, options));
    }

    get stream(): MediaStream { return this._node.stream; }
}
