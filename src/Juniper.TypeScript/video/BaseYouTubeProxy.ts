import { IFetcher } from "juniper-fetcher";
import { anyAudio, anyVideo, mediaTypeParse } from "juniper-mediatypes";
import { arrayScan, IProgress, isNullOrUndefined, isString, PriorityList } from "juniper-tslib";
import { AudioRecord, FullVideoRecord, ImageRecord, VideoRecord } from "./BaseVideoPlayer";

function isVideoOrAudio(f: YTMetadataFormat): boolean {
    return anyAudio.matches(f.content_type)
        || anyVideo.matches(f.content_type);
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

export abstract class BaseYouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
    }

    private makeVideoRecord(f: YTMetadataFormat): VideoRecord {
        const { content_type, acodec, vcodec } = f;
        const fullContentType = combineContentTypeAndCodecs(content_type, vcodec, acodec);
        return {
            contentType: fullContentType,
            url: this.makeProxyURL(f.url),
            acodec: f.acodec,
            abr: f.abr * 1024,
            asr: f.asr,
            vcodec: f.vcodec,
            vbr: f.vbr * 1024,
            fps: f.fps,
            width: f.width,
            height: f.height,
            resolution: f.width * f.height
        };
    }

    private makeAudioRecord(f: YTMetadataFormat): AudioRecord {
        const { content_type, acodec, vcodec } = f;
        const fullContentType = combineContentTypeAndCodecs(content_type, vcodec, acodec);
        return {
            contentType: fullContentType,
            url: this.makeProxyURL(f.url),
            acodec: f.acodec,
            abr: f.abr * 1024,
            asr: f.asr,
            resolution: f.abr
        };
    }

    private makeImageRecord(f: YTMetadataThumbnail): ImageRecord {
        const { content_type, url, width, height } = f;
        return {
            contentType: content_type,
            url: this.makeProxyURL(url),
            width,
            height,
            resolution: width * height
        };
    }

    async loadData(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<FullVideoRecord> {
        if (isNullOrUndefined(pageURLOrMetadata)) {
            throw new Error("must provide a YouTube URL or a YTMetadata object");
        }

        const metadata = await this.loadMetadata(pageURLOrMetadata, prog);

        const formats = new PriorityList((await Promise.all(metadata
            .formats
            .filter(isVideoOrAudio)))
            .map(f => [mediaTypeParse(f.content_type).typeName, f]));

        return {
            title: metadata.title,
            thumbnail: this.makeImageRecord(arrayScan(metadata.thumbnails, (t) => t.url === metadata.thumbnail)),
            videos: formats.get("video").map((f) => this.makeVideoRecord(f)),
            audios: formats.get("audio").map((f) => this.makeAudioRecord(f))
        };
    }

    private async loadMetadata(pageURLOrMetadata: string | YTMetadata, prog?: IProgress) {
        let metadata: YTMetadata = null;
        if (!isString(pageURLOrMetadata)) {
            prog.end(pageURLOrMetadata.title);
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

