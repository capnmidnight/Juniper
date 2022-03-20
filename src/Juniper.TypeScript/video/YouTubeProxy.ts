import { IProgress, progressSplit } from "juniper-tslib";
import { BaseYouTubeProxy } from "./BaseYouTubeProxy";
import { PlayableVideo } from "./PlayableVideo";

export class YouTubeProxy extends BaseYouTubeProxy {
    async loadVideo(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<PlayableVideo> {
        const video = new PlayableVideo();
        const progs = progressSplit(prog, 2);
        const data = await this.loadData(pageURLOrMetadata, progs.shift());
        await video.load(data, progs.shift());
        return video;
    }
}
