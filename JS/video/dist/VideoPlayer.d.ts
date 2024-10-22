import { BaseSpatializer, JuniperAudioContext } from "@juniper-lib/audio";
import { IProgress } from "@juniper-lib/progress";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord } from "./data";
export declare class VideoPlayer extends BaseVideoPlayer {
    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;
    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer);
    protected onDisposing(): void;
    private showVideo;
    load(data: FullVideoRecord, prog?: IProgress): Promise<this>;
    clear(): void;
    protected setTitle(v: string): void;
    protected loadThumbnail(data: FullVideoRecord, prog?: IProgress): Promise<void>;
}
//# sourceMappingURL=VideoPlayer.d.ts.map