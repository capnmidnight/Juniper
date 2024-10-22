import { IProgress } from "@juniper-lib/progress";
import { FullAudioRecord } from "../data";
import { IBasePlayable, MediaElementSourceEvent, MediaElementSourceEvents } from "./IPlayable";


export interface IPlayer<RecordT extends FullAudioRecord> extends IBasePlayable<MediaPlayerEvents<RecordT, IPlayer<RecordT>>> {
    data: RecordT | string;
    clear(): void;
    load(data: string | RecordT, prog?: IProgress): Promise<this>;
    volume: number;
}

class MediaPlayerEvent<T extends string, RecordT extends FullAudioRecord> extends MediaElementSourceEvent<T, IPlayer<RecordT>> {
    constructor(type: T, source: IPlayer<RecordT>) {
        super(type, source);
    }
}

export class MediaPlayerLoadingEvent<RecordT extends FullAudioRecord, PlayerT extends IPlayer<RecordT>> extends MediaPlayerEvent<"loading", RecordT> {
    constructor(source: PlayerT) {
        super("loading", source);
    }
}

export type MediaPlayerEvents<RecordT extends FullAudioRecord, PlayerT extends IPlayer<RecordT> = IPlayer<RecordT>> = MediaElementSourceEvents<PlayerT> & {
    loading: MediaPlayerLoadingEvent<RecordT, PlayerT>;
}
