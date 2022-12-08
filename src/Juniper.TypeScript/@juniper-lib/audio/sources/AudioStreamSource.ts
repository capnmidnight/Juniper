import { stringToName } from "@juniper-lib/tslib/strings/stringToName";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode } from "../context/IAudioNode";
import type { JuniperAudioContext } from "../context/JuniperAudioContext";
import type { JuniperMediaElementAudioSourceNode } from "../context/JuniperMediaElementAudioSourceNode";
import { JuniperMediaStreamAudioSourceNode } from "../context/JuniperMediaStreamAudioSourceNode";
import type { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";

export type AudioStreamSourceNode = JuniperMediaElementAudioSourceNode | JuniperMediaStreamAudioSourceNode;
const hasStreamSources = "createMediaStreamSource" in AudioContext.prototype;
const useElementSourceForUsers = !hasStreamSources;

export class AudioStreamSource extends BaseAudioSource<AudioStreamSourceNode> {

    private _stream: MediaStream = null;
    private _node: IAudioNode = null;

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer, ...effectNames: string[]) {
        super("audio-stream-source", context, spatializer, effectNames);
    }

    get stream(): MediaStream {
        return this._stream;
    }

    set stream(v: MediaStream) {
        if (v !== this.stream) {
            if (isDefined(this.stream)) {
                this.remove(this._node);
                this._node.dispose();
                this._node = null;
            }

            if (isDefined(v)) {
                this._node = new JuniperMediaStreamAudioSourceNode(
                    this.context,
                    {
                        mediaStream: v,
                        muted: !useElementSourceForUsers
                    });
                this._node.name = stringToName("media-stream-source", v.id);
                this._node.connect(this.volumeControl);
                this.add(this._node);
            }
        }
    }
}
