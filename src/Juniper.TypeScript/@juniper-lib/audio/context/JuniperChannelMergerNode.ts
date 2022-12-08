import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperChannelMergerNode
    extends JuniperAudioNode<ChannelMergerNode>
    implements ChannelMergerNode {

    constructor(context: JuniperAudioContext, options?: ChannelMergerOptions) {
        super("channel-merger", context, new ChannelMergerNode(context, options));
    }
}