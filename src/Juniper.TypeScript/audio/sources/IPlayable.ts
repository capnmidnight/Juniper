import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { PlaybackState } from "./PlaybackState";

export interface IBasePlayable<T extends MediaElementSourceEvents> extends TypedEventBase<T> {
    playbackState: PlaybackState;
    play(): Promise<void>;
    playThrough(): Promise<void>;
    pause(): void;
    stop(): void;
    restart(): void;
}

export interface IPlayable extends IBasePlayable<MediaElementSourceEvents> {
}

export class MediaElementSourceEvent<T extends string, P> extends TypedEvent<T> {
    constructor(type: T, public readonly source: P) {
        super(type);
    }
}

export class MediaElementSourceLoadedEvent<T> extends MediaElementSourceEvent<"loaded", T> {
    constructor(source: T) {
        super("loaded", source);
    }
}

export class MediaElementSourceErroredEvent<T> extends MediaElementSourceEvent<"errored", T> {
    constructor(source: T, public error: unknown) {
        super("errored", source);
    }
}

export class MediaElementSourcePlayedEvent<T> extends MediaElementSourceEvent<"played", T> {
    constructor(source: T) {
        super("played", source);
    }
}

export class MediaElementSourcePausedEvent<T> extends MediaElementSourceEvent<"paused", T> {
    constructor(source: T) {
        super("paused", source);
    }
}

export class MediaElementSourceStoppedEvent<T> extends MediaElementSourceEvent<"stopped", T> {
    constructor(source: T) {
        super("stopped", source);
    }
}

export class MediaElementSourceProgressEvent<T> extends MediaElementSourceEvent<"progress", T> {
    public value = 0;
    public total = 0;

    constructor(source: T) {
        super("progress", source);
    }
}

export interface BaseMediaElementSourceEvents<T extends IPlayable> {
    errored: MediaElementSourceErroredEvent<T>;
    loaded: MediaElementSourceLoadedEvent<T>;
    played: MediaElementSourcePlayedEvent<T>;
    paused: MediaElementSourcePausedEvent<T>;
    stopped: MediaElementSourceStoppedEvent<T>;
    progress: MediaElementSourceProgressEvent<T>;
}

export interface MediaElementSourceEvents extends BaseMediaElementSourceEvents<IPlayable> {
}