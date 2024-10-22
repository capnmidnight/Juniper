import { dispose, isDefined, stringToName } from "@juniper-lib/util";
import { TypedEvent } from "@juniper-lib/events";
import type { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperMediaStreamAudioSourceNode } from "../context/JuniperMediaStreamAudioSourceNode";
import { IAudioNode } from "../IAudioNode";
import type { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";

export class AudioSourceAddedEvent extends TypedEvent<"sourceadded"> {
    constructor(public readonly source: IAudioNode) {
        super("sourceadded");
    }
}

export type AudioSourceEvents = {
    "sourceadded": AudioSourceAddedEvent;
}

export class AudioStreamSource extends BaseAudioSource<AudioSourceEvents> {

    private _stream: MediaStream = null;
    private _node: JuniperMediaStreamAudioSourceNode = null;

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer, ...effectNames: string[]) {
        super("audio-stream-source", context, spatializer, effectNames);
    }

    get stream(): MediaStream {
        return this._stream;
    }

    set stream(mediaStream: MediaStream) {
        if (mediaStream !== this.stream) {
            if (isDefined(this.stream)) {
                this.remove(this._node);
                dispose(this._node);
                this._node = null;
            }

            if (isDefined(mediaStream)) {
                this._node = new JuniperMediaStreamAudioSourceNode(
                    this.context,
                    {
                        mediaStream: mediaStream
                    });
                this._node.name = stringToName("media-stream-source", mediaStream.id);
                this._node.connect(this.volumeControl);
                this.add(this._node);
                this.dispatchEvent(new AudioSourceAddedEvent(this));
            }
        }
    }
}
