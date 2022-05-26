import { AudioRecord } from "@juniper-lib/audio/data";
import { IFetcher } from "@juniper-lib/fetcher";
import { arrayScan, IProgress, isDefined, isNullOrUndefined, isString, PriorityList } from "@juniper-lib/tslib";
import { FullVideoRecord, ImageRecord, VideoRecord } from "./data";

const codecReplaces = new Map([
    ["vp9", "vp09.00.10.08"]
]);

function classifyFormat(f: YTMetadataFormat) {
    if (isNullOrUndefined(f.vcodec)
        || f.vcodec === "none") {
        return "audio";
    }

    return "video";
}

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
        protected readonly makeProxyURL: (path: string | URL) => URL) {
    }

    private makeVideoRecord(f: YTMetadataFormat): VideoRecord {
        const { content_type, acodec, vcodec } = f;
        const fullContentType = combineContentTypeAndCodec(content_type, vcodec);
        return {
            contentType: fullContentType,
            url: this.makeProxyURL(f.url).href,
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
            url: this.makeProxyURL(f.url).href,
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
            url: this.makeProxyURL(url).href,
            width,
            height,
            resolution: width * height
        };
    }

    async loadData(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<FullVideoRecord> {
        if (isNullOrUndefined(pageURLOrMetadata)) {
            throw new Error("must provide a YouTube URL or a YTMetadata object");
        }

        if (isDefined(prog)) {
            prog.start();
        }

        let metadata: YTMetadata = null;
        if (isString(pageURLOrMetadata)) {
            metadata = await this.fetcher
                .get(pageURLOrMetadata)
                .progress(prog)
                .object<YTMetadata>();
        }
        else {
            if (isDefined(prog)) {
                prog.end(pageURLOrMetadata.title);
            }
            metadata = pageURLOrMetadata;
        }

        const formats = new PriorityList((await Promise.all(metadata.formats))
            .map((f) => [classifyFormat(f), f]));

        const title = metadata.title;
        const thumbnails = metadata.thumbnails || [];
        const thumbnail = metadata.thumbnail && this.makeImageRecord(arrayScan(thumbnails, (t) => t.url === metadata.thumbnail));
        const videos = formats.get("video").map((f) => this.makeVideoRecord(f));
        const audios = formats.get("audio").map((f) => this.makeAudioRecord(f));

        const data: FullVideoRecord = {
            title,
            thumbnail,
            videos,
            audios
        };

        return data;
    }
}

