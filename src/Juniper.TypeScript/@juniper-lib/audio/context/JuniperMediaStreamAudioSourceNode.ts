import { srcObject } from "@juniper-lib/dom/attrs";
import { BackgroundAudio } from "@juniper-lib/dom/tags";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";

const hasStreamSources = "createMediaStreamSource" in AudioContext.prototype;

export interface JuniperMediaStreamAudioSourceOptions extends MediaStreamAudioSourceOptions {
    muted?: boolean;
}

export class JuniperMediaStreamAudioSourceNode
    extends JuniperWrappedNode<MediaStreamAudioSourceNode | MediaElementAudioSourceNode>
    implements MediaStreamAudioSourceNode {

    private readonly _stream: MediaStream;

    constructor(context: JuniperAudioContext, options: JuniperMediaStreamAudioSourceOptions) {
        let node: MediaStreamAudioSourceNode | MediaElementAudioSourceNode;
        if (hasStreamSources) {
            node = new MediaStreamAudioSourceNode(context, options);
        }
        else {
            const element = BackgroundAudio(
                true,
                options.muted,
                false,
                srcObject(options.mediaStream)
            );
            node = new MediaElementAudioSourceNode(context, {

                mediaElement: element
            });
        }

        super("media-stream-audio-source", context, node);

        this._stream = options.mediaStream;
    }

    get mediaStream(): MediaStream { return this._stream; }
}
