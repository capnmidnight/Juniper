import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";


export class JuniperMediaElementAudioSourceNode
    extends JuniperAudioNode<MediaElementAudioSourceNode>
    implements MediaElementAudioSourceNode {

    constructor(context: JuniperAudioContext, options: MediaElementAudioSourceOptions) {
        super("media-element-audio-source", context, new MediaElementAudioSourceNode(context, options));
    }

    get mediaElement(): HTMLMediaElement { return this._node.mediaElement; }
}
