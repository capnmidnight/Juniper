import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperAudioDestinationNode extends JuniperAudioNode {
    constructor(context, destination) {
        super("destination", context, destination);
    }
    get maxChannelCount() { return this._node.maxChannelCount; }
}
//# sourceMappingURL=JuniperAudioDestinationNode.js.map