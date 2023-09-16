import { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";

export class JuniperAudioDestinationNode
    extends JuniperAudioNode<AudioDestinationNode>
    implements AudioDestinationNode {

    constructor(context: JuniperAudioContext, destination: AudioDestinationNode) {
        super("destination", context, destination);
    }

    get maxChannelCount(): number { return this._node.maxChannelCount; }
}
