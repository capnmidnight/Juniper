import { src } from "juniper-dom/attrs";
import { BackgroundAudio, BackgroundVideo, mediaElementForwardEvents, mediaElementReady, Source } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { arraySortByKeyInPlace, IProgress, isDefined, PriorityList } from "juniper-tslib";

type MediaType = "video" | "audio";

export async function parseYtDlp(metadata: YTMetadata): Promise<YTBasicResult> {
    const mediaFormats = new PriorityList<MediaType, YTMetadataFormat>(metadata
        .formats
        .filter(f =>
            f.acodec !== "none" || f.vcodec !== "none")
        .map(f => [f.vcodec !== "none" ? "video" : "audio", f]));
    const videoFormats = Array.from(mediaFormats.get("video"));
    const audioFormats = Array.from(mediaFormats.get("audio"));
    const withSize = videoFormats.filter(f => isDefined(f.width) && isDefined(f.height));
    const width = withSize.map(f => f.width).reduce((a, b) => Math.max(a, b), 0);
    const height = withSize.map(f => f.height).reduce((a, b) => Math.max(a, b), 0);

    arraySortByKeyInPlace(audioFormats, f => -f.abr);
    arraySortByKeyInPlace(videoFormats, f => -(f.width * f.height));

    const result: YTBasicResult = {
        title: metadata.title,
        thumbnail: metadata.thumbnail,
        width, height,
        videos: videoFormats.map(video => {
            return {
                url: video.url,
                contentType: video.content_type,
                size: video.filesize || video.filesize_approx
            };
        }),
        audios: audioFormats.map(audio => {
            return {
                url: audio.url,
                contentType: audio.content_type,
                size: audio.filesize || audio.filesize_approx
            };
        })
    }

    return result;
}

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
    }

    protected loadVideoElement(videos: YTMediaEntry[]): Promise<HTMLVideoElement> {
        return mediaElementReady(
            BackgroundVideo(
                false,
                true,
                false,
                ...videos.map(video => Source(
                    src(this.makeProxyURL(video.url))
                ))
            )
        );
    }

    protected loadAudioElement(audios: YTMediaEntry[]): Promise<HTMLAudioElement> {
        return mediaElementReady(
            BackgroundAudio(
                false,
                true,
                false,
                ...audios.map(audio => Source(
                    src(this.makeProxyURL(audio.url))
                ))
            )
        );
    }

    async loadElements(pageURL: string, metadata?: YTMetadata, prog?: IProgress): Promise<[HTMLAudioElement, HTMLVideoElement]> {
        metadata = metadata || await this.fetcher
            .get(pageURL)
            .progress(prog)
            .object<YTMetadata>();
        const basicResult = await parseYtDlp(metadata);
        const elements = await Promise.all([
            this.loadAudioElement(basicResult.audios),
            this.loadVideoElement(basicResult.videos)
        ]);
        mediaElementForwardEvents(elements[0], elements[1]);
        return elements;
    }
}
