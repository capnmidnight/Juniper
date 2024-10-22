import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import { stringToName } from "@juniper-lib/tslib/dist/strings/stringToName";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { JuniperMediaStreamAudioSourceNode } from "../context/JuniperMediaStreamAudioSourceNode";
import { BaseAudioSource } from "./BaseAudioSource";
import { dispose } from "@juniper-lib/tslib/dist/using";
export class AudioSourceAddedEvent extends TypedEvent {
    constructor(source) {
        super("sourceadded");
        this.source = source;
    }
}
export class AudioStreamSource extends BaseAudioSource {
    constructor(context, spatializer, ...effectNames) {
        super("audio-stream-source", context, spatializer, effectNames);
        this._stream = null;
        this._node = null;
    }
    get stream() {
        return this._stream;
    }
    set stream(mediaStream) {
        if (mediaStream !== this.stream) {
            if (isDefined(this.stream)) {
                this.remove(this._node);
                dispose(this._node);
                this._node = null;
            }
            if (isDefined(mediaStream)) {
                this._node = new JuniperMediaStreamAudioSourceNode(this.context, {
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
//# sourceMappingURL=AudioStreamSource.js.map