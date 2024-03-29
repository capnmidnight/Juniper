import { AutoPlay, Controls, Loop, Muted, SrcObject } from "@juniper-lib/dom/dist/attrs";
import { display } from "@juniper-lib/dom/dist/css";
import { Audio } from "@juniper-lib/dom/dist/tags";
import { hasStreamSources } from "../util";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";

export class JuniperMediaStreamAudioSourceNode
    extends JuniperAudioNode<MediaStreamAudioSourceNode | MediaElementAudioSourceNode>
    implements MediaStreamAudioSourceNode {

    private readonly _stream: MediaStream;
    private readonly _element: HTMLAudioElement;

    constructor(context: JuniperAudioContext, options: MediaStreamAudioSourceOptions) {
        const element = Audio(
            Controls(false),
            Muted(hasStreamSources),
            AutoPlay(true),
            Loop(false),
            display("none"),
            SrcObject(options.mediaStream)
        );

        let node: MediaStreamAudioSourceNode | MediaElementAudioSourceNode;
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

    protected override onDisposing() {
        this._element.pause();
        super.onDisposing();
    }

    get mediaStream(): MediaStream { return this._stream; }
}
