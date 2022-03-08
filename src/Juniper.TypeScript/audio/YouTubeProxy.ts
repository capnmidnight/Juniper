import { src } from "juniper-dom/attrs";
import { BackgroundAudio, BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { IProgress, progressSplitWeighted } from "juniper-tslib";

export type YtDlpCallback = (pageURL: string, fetcher: IFetcher, metadata?: YTMetadata, prog?: IProgress) => Promise<YTBasicResult>;

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string,
        protected readonly queryYtDlp: YtDlpCallback) {
    }

    protected loadVideoElement(video: YTMediaEntry): Promise<HTMLVideoElement> {
        return mediaElementReady(
            BackgroundVideo(
                false,
                true,
                false,
                src(this.makeProxyURL(video.url))));
    }

    protected loadAudioElement(audio: YTMediaEntry): Promise<HTMLAudioElement> {
        return mediaElementReady(
            BackgroundAudio(
                false,
                true,
                false,
                src(this.makeProxyURL(audio.url))));
    }

    async loadElements(pageURL: string, metadata?: YTMetadata, prog?: IProgress): Promise<[HTMLAudioElement, HTMLVideoElement]> {
        const progs = progressSplitWeighted(prog, [1.000, 10.000]);
        const basicResult = await this.queryYtDlp(pageURL, this.fetcher, metadata, progs.shift());
        const elements = await Promise.all([
            this.loadAudioElement(basicResult.audio),
            this.loadVideoElement(basicResult.video)
        ]);
        mediaElementForwardEvents(elements[0], elements[1]);
        return elements;
    }
}
