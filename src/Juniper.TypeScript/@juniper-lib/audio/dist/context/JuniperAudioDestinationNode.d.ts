import { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperAudioDestinationNode extends JuniperAudioNode<AudioDestinationNode> implements AudioDestinationNode {
    constructor(context: JuniperAudioContext, destination: AudioDestinationNode);
    get maxChannelCount(): number;
}
//# sourceMappingURL=JuniperAudioDestinationNode.d.ts.map