import { IProgress } from "@juniper-lib/tslib";
import { FullAudioRecord } from "../data";
import { BaseMediaElementSourceEvents, IBasePlayable, MediaElementSourceEvent } from "./IPlayable";


export interface IPlayer extends IBasePlayable<MediaPlayerEvents> {
    data: FullAudioRecord | string;
    clear(): void;
    load(data: FullAudioRecord | string, prog?: IProgress): Promise<this>;
}

class MediaPlayerEvent<T extends string> extends MediaElementSourceEvent<T, IPlayer> {
    constructor(type: T, source: IPlayer) {
        super(type, source);
    }
}

export class MediaPlayerLoadingEvent extends MediaPlayerEvent<"loading"> {
    constructor(source: IPlayer) {
        super("loading", source);
    }
}

export interface MediaPlayerEvents extends BaseMediaElementSourceEvents<IPlayer> {
    loading: MediaPlayerLoadingEvent;
}
