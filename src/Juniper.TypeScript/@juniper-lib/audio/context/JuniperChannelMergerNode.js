import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperChannelMergerNode extends JuniperAudioNode {
    constructor(context, options) {
        super("channel-merger", context, new ChannelMergerNode(context, options));
    }
}
//# sourceMappingURL=JuniperChannelMergerNode.js.map