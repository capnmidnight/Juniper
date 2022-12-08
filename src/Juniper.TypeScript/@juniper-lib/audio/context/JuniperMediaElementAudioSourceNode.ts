import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperMediaElementAudioSourceNode
    extends JuniperWrappedNode<MediaElementAudioSourceNode>
    implements MediaElementAudioSourceNode {

    constructor(context: JuniperAudioContext, options?: MediaElementAudioSourceOptions) {
        super("media-element-audio-source", context, new MediaElementAudioSourceNode(context, options));
    }

    get mediaElement(): HTMLMediaElement { return this._node.mediaElement; }
}
