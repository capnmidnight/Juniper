import { IProgress } from "@juniper-lib/progress/IProgress";
import { FullAudioRecord } from "../data";
import { IBasePlayable, MediaElementSourceEvent, MediaElementSourceEvents } from "./IPlayable";
export interface IPlayer extends IBasePlayable<MediaPlayerEvents<IPlayer>> {
    data: FullAudioRecord | string;
    clear(): void;
    load(data: FullAudioRecord | string, prog?: IProgress): Promise<this>;
    volume: number;
}
declare class MediaPlayerEvent<T extends string> extends MediaElementSourceEvent<T, IPlayer> {
    constructor(type: T, source: IPlayer);
}
export declare class MediaPlayerLoadingEvent extends MediaPlayerEvent<"loading"> {
    constructor(source: IPlayer);
}
export type MediaPlayerEvents<T extends IPlayer = IPlayer> = MediaElementSourceEvents<T> & {
    loading: MediaPlayerLoadingEvent;
};
export {};
//# sourceMappingURL=IPlayer.d.ts.map