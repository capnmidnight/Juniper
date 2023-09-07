import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperMediaElementAudioSourceNode extends JuniperAudioNode<MediaElementAudioSourceNode> implements MediaElementAudioSourceNode {
    constructor(context: JuniperAudioContext, options: MediaElementAudioSourceOptions);
    get mediaElement(): HTMLMediaElement;
}
//# sourceMappingURL=JuniperMediaElementAudioSourceNode.d.ts.map