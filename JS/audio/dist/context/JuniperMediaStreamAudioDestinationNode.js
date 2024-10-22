import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperMediaStreamAudioDestinationNode extends JuniperAudioNode {
    constructor(context, options) {
        super("media-stream-audio-destination", context, new MediaStreamAudioDestinationNode(context, options));
    }
    get stream() { return this._node.stream; }
}
//# sourceMappingURL=JuniperMediaStreamAudioDestinationNode.js.map