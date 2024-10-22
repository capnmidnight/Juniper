import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperConvolverNode extends JuniperAudioNode<ConvolverNode> implements ConvolverNode {
    constructor(context: JuniperAudioContext, options?: ConvolverOptions);
    get buffer(): AudioBuffer;
    set buffer(v: AudioBuffer);
    get normalize(): boolean;
    set normalize(v: boolean);
}
//# sourceMappingURL=JuniperConvolverNode.d.ts.map