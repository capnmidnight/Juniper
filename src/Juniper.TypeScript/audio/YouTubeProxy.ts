import { IFetcher } from "juniper-fetcher";
import { anyAudio, anyVideo, mediaTypeParse } from "juniper-mediatypes";
import { IProgress, isNullOrUndefined, isString, PriorityList } from "juniper-tslib";
import { PlayableVideo } from "./sources/PlayableVideo";

function isVideoOrAudio(f: YTMetadataFormat): boolean {
    return anyAudio.matches(f.content_type)
        || anyVideo.matches(f.content_type);
}

export interface MediaRecord {
    url: string;
    size: number;
    content_type: string;
    resolution: number;
    width?: number;
    height?: number;
    data?: YTMetadataFormat;
}

export function combineContentTypeAndCodecs(content_type: string, ...codecs: string[]): string {
    const parts = [content_type];

    codecs = codecs.filter(c => c !== "none");

    if (codecs.length > 0) {
        const asterisk = codecs.filter(c => encodeURI(c) !== c).length > 0
            ? "*"
            : "";
        parts.push(`codecs${asterisk}="${codecs.map(encodeURI).join(',')}"`);
    }

    return parts.join(";");
}

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
    }

    private makeBasicFormat(f: YTMetadataFormat): MediaRecord {
        const { content_type, acodec, vcodec } = f;
        const fullContentType = combineContentTypeAndCodecs(content_type, vcodec, acodec);
        return {
            data: f,
            content_type: fullContentType,
            width: f.width,
            height: f.height,
            resolution: (f.width * f.height) || f.abr,
            size: f.filesize || f.filesize_approx,
            url: this.makeProxyURL(f.url)
        };
    }

    async load(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<PlayableVideo> {

        if (isNullOrUndefined(pageURLOrMetadata)) {
            throw new Error("must provide a YouTube URL or a YTMetadata object");
        }

        const metadata = await this.loadMetadata(pageURLOrMetadata, prog);

        const formats = new PriorityList((await Promise.all(metadata
            .formats
            .filter(isVideoOrAudio)
            .map(f => this.makeBasicFormat(f))))
            .map(f => [mediaTypeParse(f.content_type).typeName, f]));

        return new PlayableVideo(
            metadata.title,
            formats.get("video"),
            formats.get("audio"),
            this.makeProxyURL(metadata.thumbnail));
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
}