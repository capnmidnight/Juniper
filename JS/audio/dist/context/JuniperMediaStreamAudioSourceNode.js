import { Audio, AutoPlay, Controls, display, Loop, Muted, SrcObject } from "@juniper-lib/dom";
import { hasStreamSources } from "../util";
import { JuniperAudioNode } from "./JuniperAudioNode";
export class JuniperMediaStreamAudioSourceNode extends JuniperAudioNode {
    constructor(context, options) {
        const element = Audio(Controls(false), Muted(hasStreamSources), AutoPlay(true), Loop(false), display("none"), SrcObject(options.mediaStream));
        let node;
        if (hasStreamSources) {
            node = new MediaStreamAudioSourceNode(context, options);
        }
        else {
            node = new MediaElementAudioSourceNode(context, {
                mediaElement: element
            });
        }
        super("media-stream-audio-source", context, node);
        this._stream = options.mediaStream;
        this._element = element;
    }
    onDisposing() {
        this._element.pause();
        super.onDisposing();
    }
    get mediaStream() { return this._stream; }
}
//# sourceMappingURL=JuniperMediaStreamAudioSourceNode.js.map