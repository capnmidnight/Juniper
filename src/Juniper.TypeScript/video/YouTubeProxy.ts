import { AudioRecord } from "juniper-audio/data";
import { IFetcher } from "juniper-fetcher";
import { anyAudio, anyVideo, mediaTypeParse } from "juniper-mediatypes";
import { Video_Vendor_Mpeg_Dash_Mpd } from "juniper-mediatypes/video";
import { arrayScan, IProgress, isDefined, isNullOrUndefined, isString, PriorityList } from "juniper-tslib";
import { FullVideoRecord, ImageRecord, VideoRecord } from "./data";

function isVideoOrAudio(f: YTMetadataFormat): boolean {
    return (anyAudio.matches(f.content_type)
        || anyVideo.matches(f.content_type))
        && !Video_Vendor_Mpeg_Dash_Mpd.matches(f.content_type);
}

const codecReplaces = new Map([
    ["vp9", "vp09.00.10.08"]
]);

export function combineContentTypeAndCodec(content_type: string, codec: string): string {
    const parts = [content_type];

    if (isDefined(codec)
        && codec.length > 0
        && codec !== "none") {
        codec = codecReplaces.get(codec) || codec;
    }


    if (isDefined(codec)
        && codec.length > 0
        && codec !== "none") {
        const asterisk = encodeURI(codec) !== codec
            ? "*"
            : "";

        parts.push(`codecs${asterisk}="${codec}"`);
    }

    return parts.join(";");
}

export class YouTubeProxy {
    constructor(
        protected fetcher: IFetcher,
        protected readonly makeProxyURL: (path: string) => string) {
    }

    private makeVideoRecord(f: YTMetadataFormat): VideoRecord {
        const { content_type, acodec, vcodec } = f;
        const fullContentType = combineContentTypeAndCodec(content_type, vcodec);
        return {
            contentType: fullContentType,
            url: this.makeProxyURL(f.url),
            acodec,
            abr: f.abr * 1024,
            asr: f.asr,
            vcodec,
            vbr: f.vbr * 1024,
            fps: f.fps,
            width: f.width,
            height: f.height,
            resolution: f.width * f.height
        };
    }

    private makeAudioRecord(f: YTMetadataFormat): AudioRecord {
        const { content_type, acodec } = f;
        const fullContentType = combineContentTypeAndCodec(content_type, acodec);
        return {
            contentType: fullContentType,
            url: this.makeProxyURL(f.url),
            acodec,
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

        let metadata: YTMetadata = null;
        if (isString(pageURLOrMetadata)) {
            metadata = await this.fetcher
                .get(pageURLOrMetadata)
                .progress(prog)
                .object<YTMetadata>();
        }
        else {
            prog.end(pageURLOrMetadata.title);
            metadata = pageURLOrMetadata;
        }

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
}

