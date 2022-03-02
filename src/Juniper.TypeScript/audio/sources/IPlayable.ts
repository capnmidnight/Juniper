import { TypedEvent, TypedEventBase } from "juniper-tslib";

export type PlaybackState = "playing" | "paused" | "stopped" | "errored";

export interface IPlayable extends TypedEventBase<MediaElementSourceEvents> {
    playbackState: PlaybackState;
    play(): Promise<void>;
    playThrough(): Promise<void>;
    pause(): void;
    stop(): void;
    restart(): void;
}

class MediaElementSourceEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly source: IPlayable) {
        super(type);
    }
}

export class MediaElementSourcePlayedEvent extends MediaElementSourceEvent<"played"> {
    constructor(source: IPlayable) {
        super("played", source);
    }
}

export class MediaElementSourcePausedEvent extends MediaElementSourceEvent<"paused"> {
    constructor(source: IPlayable) {
        super("paused", source);
    }
}

export class MediaElementSourceStoppedEvent extends MediaElementSourceEvent<"stopped"> {
    constructor(source: IPlayable) {
        super("stopped", source);
    }
}

export class MediaElementSourceEndedEvent extends MediaElementSourceEvent<"ended"> {
    constructor(source: IPlayable) {
        super("ended", source);
    }
}

export class MediaElementSourceProgressEvent extends MediaElementSourceEvent<"progress"> {
    public value = 0;
    public total = 0;

    constructor(source: IPlayable) {
        super("progress", source);
    }
}

export interface MediaElementSourceEvents {
    played: MediaElementSourcePlayedEvent;
    paused: MediaElementSourcePausedEvent;
    stopped: MediaElementSourceStoppedEvent;
    ended: MediaElementSourceEndedEvent;
    progress: MediaElementSourceProgressEvent;
}