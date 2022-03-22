import { IProgress, TypedEvent, TypedEventBase } from "juniper-tslib";
import { FullAudioRecord } from "../data";

export type PlaybackState = "empty"
    | "loading"
    | "errored"
    | "stopped"
    | "paused"
    | "playing";

export interface IPlayable extends TypedEventBase<MediaElementSourceEvents> {
    playbackState: PlaybackState;
    play(): Promise<void>;
    playThrough(): Promise<void>;
    pause(): void;
    stop(): void;
    restart(): void;
}

export interface IPlayer<T extends FullAudioRecord> extends IPlayable {
    title: string;
    data: T;
    clear(): void;
    load(data: T, prog?: IProgress): Promise<this>;
}

class MediaElementSourceEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly source: IPlayable) {
        super(type);
    }
}

export class MediaElementSourceLoadingEvent extends MediaElementSourceEvent<"loading"> {
    constructor(source: IPlayable) {
        super("loading", source);
    }
}

export class MediaElementSourceLoadedEvent extends MediaElementSourceEvent<"loaded"> {
    constructor(source: IPlayable) {
        super("loaded", source);
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

export class MediaElementSourceProgressEvent extends MediaElementSourceEvent<"progress"> {
    public value = 0;
    public total = 0;

    constructor(source: IPlayable) {
        super("progress", source);
    }
}

export interface MediaElementSourceEvents {
    loading: MediaElementSourceLoadingEvent;
    loaded: MediaElementSourceLoadedEvent;
    played: MediaElementSourcePlayedEvent;
    paused: MediaElementSourcePausedEvent;
    stopped: MediaElementSourceStoppedEvent;
    progress: MediaElementSourceProgressEvent;
}