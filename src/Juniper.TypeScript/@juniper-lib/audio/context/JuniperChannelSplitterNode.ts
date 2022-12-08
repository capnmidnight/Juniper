import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperChannelSplitterNode
    extends JuniperWrappedNode<ChannelSplitterNode>
    implements ChannelSplitterNode {

    constructor(context: JuniperAudioContext, options?: ChannelSplitterOptions) {
        super("channel-splitter", context, new ChannelSplitterNode(context, options));
    }
}
