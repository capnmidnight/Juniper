import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperDynamicsCompressorNode extends JuniperAudioNode<DynamicsCompressorNode> implements DynamicsCompressorNode {
    readonly attack: IAudioParam;
    readonly knee: IAudioParam;
    readonly ratio: IAudioParam;
    readonly release: IAudioParam;
    readonly threshold: IAudioParam;
    constructor(context: JuniperAudioContext, options?: DynamicsCompressorOptions);
    get reduction(): number;
}
//# sourceMappingURL=JuniperDynamicsCompressorNode.d.ts.map