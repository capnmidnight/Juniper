import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperChannelSplitterNode extends JuniperAudioNode {
    constructor(context, options) {
        super("channel-splitter", context, new ChannelSplitterNode(context, options));
    }
}
//# sourceMappingURL=JuniperChannelSplitterNode.js.map