import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperChannelMergerNode
    extends JuniperWrappedNode<ChannelMergerNode>
    implements ChannelMergerNode {

    constructor(context: JuniperAudioContext, options?: ChannelMergerOptions) {
        super("channel-merger", context, new ChannelMergerNode(context, options));
    }
}