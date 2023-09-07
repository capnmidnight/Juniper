import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperStereoPannerNode extends JuniperAudioNode<StereoPannerNode> implements StereoPannerNode {
    readonly pan: IAudioParam;
    constructor(context: JuniperAudioContext, options?: StereoPannerOptions);
}
//# sourceMappingURL=JuniperStereoPannerNode.d.ts.map