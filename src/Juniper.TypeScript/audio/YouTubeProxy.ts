import { autoPlay, controls, loop, muted, playsInline, src, title, type } from "juniper-dom/attrs";
import { Audio, ElementChild, Source, Video } from "juniper-dom/tags";
import { IFetcher } from "juniper-fetcher";
import { anyAudio, anyVideo, mediaTypeParse } from "juniper-mediatypes";
import { Video_Vendor_Mpeg_Dash_Mpd } from "juniper-mediatypes/video";
import { arraySortByKeyInPlace, IProgress, isDefined, isNullOrUndefined, isString, once, PriorityList, progressSplit } from "juniper-tslib";

function makeBasicFormat(f: YTMetadataFormat): [string, YTMediaEntry] {
    const { content_type, acodec, vcodec } = f;
    const subType = mediaTypeParse(content_type).typeName;
    const codecs = [vcodec, acodec]
        .filter(c => c !== "none")
        .map(c => "codec=" + c);
    const parts = [content_type, ...codecs];

    return [subType, {
        content_type: parts.join("; "),
        width: f.width,
        height: f.height,
        resolution: (f.width * f.height) || f.abr,
        size: f.filesize || f.filesize_approx,
        url: f.url
    }];
}

function isVideoOrAudio(f: YTMetadataFormat): boolean {
    return anyAudio.matches(f.content_type)
        || (anyVideo.matches(f.content_type)
            && !Video_Vendor_Mpeg_Dash_Mpd.matches(f.content_type));
}

function isVideoXorAudio(f: YTMetadataFormat): boolean {
    return f.acodec === "none"
        || f.vcodec === "none";
}

export type MediaEventForwardingDirection = "audio-to-video"
    | "video-to-audio";

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
    }

    async loadElements(pageURLOrMetadata: string | YTMetadata, fwdDir: MediaEventForwardingDirection = "audio-to-video", prog?: IProgress): Promise<[HTMLAudioElement, HTMLVideoElement, HTMLImageElement]> {

        if (isNullOrUndefined(pageURLOrMetadata)) {
            throw new Error("must provide a YouTube URL or a YTMetadata object");
        }

        const progs = progressSplit(prog, 4);

        const metadata = await this.loadMetadata(pageURLOrMetadata, progs.shift());

        const mediaFormats = new PriorityList(
            metadata
                .formats
                .filter(isVideoOrAudio)
                .filter(isVideoXorAudio)
                .map(makeBasicFormat)
        );

        const videos = mediaFormats.get("video");
        const audios = mediaFormats.get("audio");

        arraySortByKeyInPlace(videos, f => -f.resolution);
        arraySortByKeyInPlace(audios, f => -f.resolution);

        const showVideoControls = fwdDir === "video-to-audio";

        const [audio, video, thumbnail] = await Promise.all([
            this.loadAudio(audios, metadata.title, !showVideoControls, progs.shift()),
            this.loadVideo(videos, metadata.title, showVideoControls, progs.shift()),
            this.loadImage(metadata.thumbnail, metadata.title, progs.shift())
        ]);

        if (isDefined(audio)) {
            const from = showVideoControls
                ? video
                : audio;
            const to = showVideoControls
                ? audio
                : video;

            const play = () => {
                to.currentTime = from.currentTime;
                to.play();
            };

            const pause = () =>
                to.pause();

            const copyCurrentTime = () =>
                to.currentTime = from.currentTime;

            const updateTime = () => {
                const delta = to.currentTime - from.currentTime;
                if (Math.abs(delta) > 0.25) {
                    to.currentTime = from.currentTime;
                }
            };

            from.addEventListener("play", play);
            from.addEventListener("pause", pause);
            from.addEventListener("seeked", copyCurrentTime);
            from.addEventListener("timeupdate", updateTime);
        }

        return [audio, video, thumbnail];
    }

    private async loadMetadata(pageURLOrMetadata: string | YTMetadata, prog?: IProgress) {
        let metadata: YTMetadata = null;
        if (!isString(pageURLOrMetadata)) {
            prog.report(1, 1, pageURLOrMetadata.title);
            metadata = pageURLOrMetadata;
        }
        else {
            metadata = await this.fetcher
                .get(pageURLOrMetadata)
                .progress(prog)
                .object<YTMetadata>();
        }
        return metadata;
    }

    private async loadMediaElement<T extends HTMLMediaElement>(thunk: (...rest: ElementChild[]) => T, sources: YTMediaEntry[], t: string, showControls: boolean, muteAudio: boolean, prog?: IProgress): Promise<T> {
        prog.report(0, 1, t);

        const element = thunk(
            title(t),
            playsInline(true),
            controls(showControls),
            muted(muteAudio),
            autoPlay(false),
            loop(false),
            ...sources.map(f => Source(
                type(f.content_type),
                src(this.makeProxyURL(f.url))
            ))
        );

        await once<HTMLMediaElementEventMap, "canplay">(element, "canplay");

        prog.report(1, 1, t);

        return element;
    }

    private loadAudio(sources: YTMediaEntry[], t: string, showControls: boolean, prog?: IProgress): Promise<HTMLAudioElement> {
        prog.report(0, 1, t);

        if (sources.length === 0) {
            prog.report(1, 1, t);
            return null;
        }

        return this.loadMediaElement(Audio, sources, t, showControls, false, prog);
    }

    private async loadVideo(sources: YTMediaEntry[], t: string, showControls: boolean, prog?: IProgress): Promise<HTMLVideoElement> {
        prog.report(0, 1, t);

        return this.loadMediaElement(Video, sources, t, showControls, true, prog);
    }

    private async loadImage(url: string, title: string, prog?: IProgress): Promise<HTMLImageElement> {
        const { content: thumbnail } = await this.fetcher
            .get(this.makeProxyURL(url))
            .progress(prog)
            .image();

        thumbnail.title = title;

        return thumbnail;
    }
}