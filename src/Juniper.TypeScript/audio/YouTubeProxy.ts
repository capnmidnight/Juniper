import { src } from "juniper-dom/attrs";
import { BackgroundAudio, BackgroundVideo, mediaElementForwardEvents, mediaElementReady } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { arraySortByKeyInPlace, IProgress, isDefined, isNullOrUndefined, singleton } from "juniper-tslib";

const testAudio = singleton("Juniper.Audio.TestAudio", () => BackgroundAudio(false, false, false));
const testVideo = singleton("Juniper.Audio.TestVideo", () => BackgroundVideo(false, false, false));
const cache = singleton("Juniper.Audio.TestResults", () => new Map<string, Promise<MediaCapabilitiesDecodingInfo>>());

export async function parseYtDlp(metadata: YTMetadata): Promise<YTBasicResult> {
    const mediaFormats = metadata
        .formats
        .filter(f =>
            f.acodec !== "none" || f.vcodec !== "none");
    const formatTests = await Promise.all(
        mediaFormats.map(testFormat)
    );
    const formatSet = new Set(formatTests
        .filter(f => f[0].supported)
        .map(f => f[1]));
    const formats = Array.from(formatSet.values());
    const audioFormats = formats.filter(f => f.acodec !== "none");
    const videoFormats = formats.filter(f => f.vcodec !== "none");

    arraySortByKeyInPlace(audioFormats, f => -f.abr);
    arraySortByKeyInPlace(videoFormats, f => -(f.width * f.height));

    const audio = audioFormats[0];
    const video = videoFormats[0];

    const result: YTBasicResult = {
        title: metadata.title,
        thumbnail: metadata.thumbnail
    };

    if (isDefined(audio)) {
        result.audio = {
            url: audio.url,
            contentType: audio.content_type,
            size: audio.filesize || audio.filesize_approx
        };
    }

    if (isDefined(video)) {
        result.video = {
            url: video.url,
            contentType: video.content_type,
            size: video.filesize || video.filesize_approx
        };

        result.width = video.width;
        result.height = video.height;
    }

    return result;
}

async function testFormat(format: YTMetadataFormat): Promise<[MediaCapabilitiesDecodingInfo, YTMetadataFormat]> {
    let videoType: string;
    let audioType: string;

    if (format.acodec !== "none" && format.vcodec !== "none") {
        videoType = `${format.content_type}; codecs="${format.vcodec}, ${format.acodec}"`;
        audioType = `${format.content_type}; codecs=${format.acodec}`
    }
    else if (format.acodec !== "none") {
        videoType = null;
        audioType = `${format.content_type}; codecs=${format.acodec}`;
    }
    else if (format.vcodec !== "none") {
        videoType = `${format.content_type}; codecs=${format.vcodec}`;
        audioType = null;
    }

    const options = makeOptions(videoType, audioType, format);
    const key = JSON.stringify(options);

    let capabilitiesTask = cache.get(key);
    if (isNullOrUndefined(capabilitiesTask)) {
        cache.set(key, capabilitiesTask = getCaps(options));
    }

    const capabilities = await capabilitiesTask;

    return [capabilities, format];
}


function makeOptions(videoType: string, audioType: string, format: YTMetadataFormat): MediaDecodingConfiguration {
    const opts: MediaDecodingConfiguration = {
        type: "media-source"
    };

    if (videoType) {
        opts.video = {
            contentType: videoType,
            bitrate: format.vbr,
            framerate: format.fps,
            width: format.width,
            height: format.height
        };
    }

    if (audioType) {
        opts.audio = {
            contentType: audioType,
            bitrate: format.abr,
            samplerate: format.asr
        };
    }
    return opts;
}

async function getCaps(opts: MediaDecodingConfiguration): Promise<MediaCapabilitiesDecodingInfo> {
    if (isDefined(opts.video) || isDefined(opts.audio)) {
        if (!navigator.mediaCapabilities) {
            const status = await (opts.video
                ? testVideo.canPlayType(opts.video.contentType)
                : testAudio.canPlayType(opts.audio.contentType));
            return {
                supported: status === "probably" || status === "maybe",
                smooth: status === "probably",
                powerEfficient: status === "probably"
            };
        }
        else {
            try {
                return await navigator.mediaCapabilities.decodingInfo(opts);
            }
            catch {
                return {
                    supported: false,
                    powerEfficient: false,
                    smooth: false
                };
            }
        }
    }
    else {
        return {
            supported: false,
            powerEfficient: false,
            smooth: false
        };
    }
}

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
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
        metadata = metadata || await this.fetcher
            .get(pageURL)
            .progress(prog)
            .object<YTMetadata>();
        const basicResult = await parseYtDlp(metadata);
        const elements = await Promise.all([
            this.loadAudioElement(basicResult.audio),
            this.loadVideoElement(basicResult.video)
        ]);
        mediaElementForwardEvents(elements[0], elements[1]);
        return elements;
    }
}
