import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperDelayNode extends JuniperAudioNode<DelayNode> implements DelayNode {
    readonly delayTime: IAudioParam;
    constructor(context: JuniperAudioContext, options?: DelayOptions);
}
//# sourceMappingURL=JuniperDelayNode.d.ts.map