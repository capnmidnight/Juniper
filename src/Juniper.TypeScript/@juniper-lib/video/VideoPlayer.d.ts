import { JuniperAudioContext } from "@juniper-lib/audio/context/JuniperAudioContext";
import { BaseSpatializer } from "@juniper-lib/audio/spatializers/BaseSpatializer";
import { ErsatzElement } from "@juniper-lib/dom/tags";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord } from "./data";
export declare class VideoPlayer extends BaseVideoPlayer implements ErsatzElement {
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