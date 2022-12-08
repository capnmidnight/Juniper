import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperMediaStreamAudioDestinationNode
    extends JuniperAudioNode<MediaStreamAudioDestinationNode>
    implements MediaStreamAudioDestinationNode {

    constructor(context: JuniperAudioContext, options?: AudioNodeOptions) {
        super("media-stream-audio-destination", context, new MediaStreamAudioDestinationNode(context, options));
    }

    get stream(): MediaStream { return this._node.stream; }
}
