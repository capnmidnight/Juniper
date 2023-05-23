import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { dispose } from "@juniper-lib/tslib/using";
import { IAudioNode } from "../IAudioNode";
import { JuniperAudioBufferSourceNode } from "../context/JuniperAudioBufferSourceNode";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { AudioSourceAddedEvent, AudioSourceEvents } from "./AudioStreamSource";
import { BaseAudioSource } from "./BaseAudioSource";


export class AudioBufferSource extends BaseAudioSource<AudioSourceEvents> {

    private _buffer: AudioBuffer = null;
    private _node: IAudioNode = null;

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer, ...effectNames: string[]) {
        super("audio-buffer-source", context, spatializer, effectNames);
    }

    get buffer(): AudioBuffer {
        return this._buffer;
    }

    set buffer(buffer: AudioBuffer) {
        if (buffer !== this.buffer) {
            if (isDefined(this.buffer)) {
                this.remove(this._node);
                dispose(this._node);
                this._node = null;
            }

            if (isDefined(buffer)) {
                this._node = new JuniperAudioBufferSourceNode(
                    this.context,
                    {
                        buffer
                    });
                this._node.connect(this.volumeControl);
                this.add(this._node);
                this.dispatchEvent(new AudioSourceAddedEvent(this));
            }
        }
    }
}
