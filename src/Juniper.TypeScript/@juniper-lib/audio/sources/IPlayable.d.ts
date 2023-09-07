import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { PlaybackState } from "./PlaybackState";
export interface IBasePlayable<T extends MediaElementSourceEvents> extends TypedEventTarget<T> {
    playbackState: PlaybackState;
    play(): Promise<void>;
    playThrough(): Promise<void>;
    pause(): void;
    stop(): void;
    restart(): void;
}
export type IPlayable = IBasePlayable<MediaElementSourceEvents>;
export declare class MediaElementSourceEvent<T extends string, P extends IPlayable> extends TypedEvent<T> {
    readonly source: P;
    constructor(type: T, source: P);
}
export declare class MediaElementSourceLoadedEvent<T extends IPlayable> extends MediaElementSourceEvent<"loaded", T> {
    constructor(source: T);
}
export declare class MediaElementSourceErroredEvent<T extends IPlayable> extends MediaElementSourceEvent<"errored", T> {
    error: unknown;
    constructor(source: T, error: unknown);
}
export declare class MediaElementSourcePlayedEvent<T extends IPlayable> extends MediaElementSourceEvent<"played", T> {
    constructor(source: T);
}
export declare class MediaElementSourcePausedEvent<T extends IPlayable> extends MediaElementSourceEvent<"paused", T> {
    constructor(source: T);
}
export declare class MediaElementSourceStoppedEvent<T extends IPlayable> extends MediaElementSourceEvent<"stopped", T> {
    constructor(source: T);
}
export declare class MediaElementSourceProgressEvent<T extends IPlayable> extends MediaElementSourceEvent<"progress", T> {
    value: number;
    total: number;
    constructor(source: T);
}
export type MediaElementSourceEvents<T extends IPlayable = IPlayable> = {
    errored: MediaElementSourceErroredEvent<T>;
    loaded: MediaElementSourceLoadedEvent<T>;
    played: MediaElementSourcePlayedEvent<T>;
    paused: MediaElementSourcePausedEvent<T>;
    stopped: MediaElementSourceStoppedEvent<T>;
    progress: MediaElementSourceProgressEvent<T>;
};
//# sourceMappingURL=IPlayable.d.ts.map