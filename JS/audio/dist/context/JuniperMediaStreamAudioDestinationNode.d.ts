import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperMediaStreamAudioDestinationNode extends JuniperAudioNode<MediaStreamAudioDestinationNode> implements MediaStreamAudioDestinationNode {
    constructor(context: JuniperAudioContext, options?: AudioNodeOptions);
    get stream(): MediaStream;
}
//# sourceMappingURL=JuniperMediaStreamAudioDestinationNode.d.ts.map