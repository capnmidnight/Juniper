import { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";

export class JuniperAudioDestinationNode
    extends JuniperWrappedNode<AudioDestinationNode>
    implements AudioDestinationNode {

    constructor(context: JuniperAudioContext, destination: AudioDestinationNode) {
        super("destination", context, destination);
    }

    get maxChannelCount(): number { return this._node.maxChannelCount; }
}
