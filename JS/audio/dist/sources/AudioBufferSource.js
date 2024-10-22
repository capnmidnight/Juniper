import { dispose, isDefined } from "@juniper-lib/util";
import { JuniperAudioBufferSourceNode } from "../context/JuniperAudioBufferSourceNode";
import { AudioSourceAddedEvent } from "./AudioStreamSource";
import { BaseAudioSource } from "./BaseAudioSource";
export class AudioBufferSource extends BaseAudioSource {
    constructor(context, spatializer, ...effectNames) {
        super("audio-buffer-source", context, spatializer, effectNames);
        this._buffer = null;
        this._node = null;
    }
    get buffer() {
        return this._buffer;
    }
    set buffer(buffer) {
        if (buffer !== this.buffer) {
            if (isDefined(this.buffer)) {
                this.remove(this._node);
                dispose(this._node);
                this._node = null;
            }
            if (isDefined(buffer)) {
                this._node = new JuniperAudioBufferSourceNode(this.context, {
                    buffer
                });
                this._node.connect(this.volumeControl);
                this.add(this._node);
                this.dispatchEvent(new AudioSourceAddedEvent(this));
            }
        }
    }
}
//# sourceMappingURL=AudioBufferSource.js.map