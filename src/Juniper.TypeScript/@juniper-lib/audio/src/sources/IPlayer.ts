import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { FullAudioRecord } from "../data";
import { IBasePlayable, MediaElementSourceEvent, MediaElementSourceEvents } from "./IPlayable";


export interface IPlayer extends IBasePlayable<MediaPlayerEvents<IPlayer>> {
    data: FullAudioRecord | string;
    clear(): void;
    load(data: FullAudioRecord | string, prog?: IProgress): Promise<this>;
    volume: number;
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

export type MediaPlayerEvents<T extends IPlayer = IPlayer> = MediaElementSourceEvents<T> & {
    loading: MediaPlayerLoadingEvent;
}
