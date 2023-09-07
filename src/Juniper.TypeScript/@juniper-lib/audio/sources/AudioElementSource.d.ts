import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperMediaElementAudioSourceNode } from "../context/JuniperMediaElementAudioSourceNode";
import type { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { BaseAudioSource } from "./BaseAudioSource";
import { IPlayable, MediaElementSourceEvents } from "./IPlayable";
import { PlaybackState } from "./PlaybackState";
type AudioElementSourceEventMap = MediaElementSourceEvents & {
    disposing: TypedEvent<"disposing">;
};
export declare class AudioElementSource extends BaseAudioSource<AudioElementSourceEventMap> implements IPlayable {
    private readonly loadEvt;
    private readonly playEvt;
    private readonly pauseEvt;
    private readonly stopEvt;
    private readonly progEvt;
    readonly audio: HTMLMediaElement;
    constructor(context: JuniperAudioContext, source: JuniperMediaElementAudioSourceNode, randomizeStart: boolean, randomizePitch: boolean, spatializer: BaseSpatializer, ...effectNames: string[]);
    onDisposing(): void;
    get playbackState(): PlaybackState;
    play(): Promise<void>;
    playThrough(): Promise<void>;
    pause(): void;
    stop(): void;
    restart(): Promise<void>;
}
export {};
//# sourceMappingURL=AudioElementSource.d.ts.map