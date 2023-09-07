import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperGainNode extends JuniperAudioNode<GainNode> implements GainNode {
    readonly gain: IAudioParam;
    constructor(context: JuniperAudioContext, options?: GainOptions);
}
//# sourceMappingURL=JuniperGainNode.d.ts.map