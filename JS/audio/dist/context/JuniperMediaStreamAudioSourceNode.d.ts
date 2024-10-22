import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperMediaStreamAudioSourceNode extends JuniperAudioNode<MediaStreamAudioSourceNode | MediaElementAudioSourceNode> implements MediaStreamAudioSourceNode {
    private readonly _stream;
    private readonly _element;
    constructor(context: JuniperAudioContext, options: MediaStreamAudioSourceOptions);
    protected onDisposing(): void;
    get mediaStream(): MediaStream;
}
//# sourceMappingURL=JuniperMediaStreamAudioSourceNode.d.ts.map