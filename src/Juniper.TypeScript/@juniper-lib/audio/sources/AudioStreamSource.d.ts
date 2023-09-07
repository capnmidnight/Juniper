import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import type { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IAudioNode } from "../IAudioNode";
import type { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";
export declare class AudioSourceAddedEvent extends TypedEvent<"sourceadded"> {
    readonly source: IAudioNode;
    constructor(source: IAudioNode);
}
export type AudioSourceEvents = {
    "sourceadded": AudioSourceAddedEvent;
};
export declare class AudioStreamSource extends BaseAudioSource<AudioSourceEvents> {
    private _stream;
    private _node;
    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer, ...effectNames: string[]);
    get stream(): MediaStream;
    set stream(mediaStream: MediaStream);
}
//# sourceMappingURL=AudioStreamSource.d.ts.map