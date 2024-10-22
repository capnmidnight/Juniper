import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperMediaElementAudioSourceNode extends JuniperAudioNode {
    constructor(context, options) {
        super("media-element-audio-source", context, new MediaElementAudioSourceNode(context, options));
    }
    get mediaElement() { return this._node.mediaElement; }
}
//# sourceMappingURL=JuniperMediaElementAudioSourceNode.js.map