import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperChannelSplitterNode
    extends JuniperAudioNode<ChannelSplitterNode>
    implements ChannelSplitterNode {

    constructor(context: JuniperAudioContext, options?: ChannelSplitterOptions) {
        super("channel-splitter", context, new ChannelSplitterNode(context, options));
    }
}
