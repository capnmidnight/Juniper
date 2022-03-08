import { src } from "juniper-dom/attrs";
import { BackgroundAudio, BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, progressSplitWeighted } from "juniper-tslib";

export type YtDlpCallback = (pageUrl: string, fetcher: IFetcher, prog?: IProgress) => Promise<YTBasicResult>;

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyUrl: (path: string) => string,
        protected readonly queryYtDlp: YtDlpCallback) {
    }

    protected loadVideoElement(vidLoc: YTMediaEntry): Promise<HTMLVideoElement> {
        return mediaElementReady(
            BackgroundVideo(
                false,
                true,
                false,
                src(this.makeProxyUrl(vidLoc.url))));
    }

    protected loadAudioElement(audLoc: YTMediaEntry): Promise<HTMLAudioElement> {
        return mediaElementReady(
            BackgroundAudio(
                false,
                true,
                false,
                src(this.makeProxyUrl(audLoc.url))));
    }

    async loadElements(pageUrl: string, prog?: IProgress): Promise<[HTMLVideoElement, HTMLAudioElement]> {
        const progs = progressSplitWeighted(prog, [1.000, 10.000]);
        const { video: vidLoc, audio: audLoc } = await this.queryYtDlp(pageUrl, this.fetcher, progs.shift());
        const elements = await Promise.all([
            this.loadVideoElement(vidLoc),
            this.loadAudioElement(audLoc)
        ]);
        mediaElementForwardEvents(elements[0], elements[1]);
        return elements;
    }
}
