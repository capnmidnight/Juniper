import { IProgress, progressSplit } from "juniper-tslib";
import { BaseYouTubeProxy } from "./BaseYouTubeProxy";
import { VideoPlayer } from "./VideoPlayer";

export class YouTubeProxy extends BaseYouTubeProxy {
    async loadVideo(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<VideoPlayer> {
        const video = new VideoPlayer();
        const progs = progressSplit(prog, 2);
        const data = await this.loadData(pageURLOrMetadata, progs.shift());
        await video.load(data, progs.shift());
        return video;
    }
}
