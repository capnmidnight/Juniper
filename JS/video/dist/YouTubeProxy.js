import { isNullOrUndefined, isDefined, isString, arrayScan } from "@juniper-lib/util";
import { PriorityList } from "@juniper-lib/collections";
import { unwrapResponse } from "@juniper-lib/fetcher";
export function isYouTube(url) {
    return url.hostname === "www.youtube.com"
        || url.hostname === "youtube.com"
        || url.hostname === "youtu.be";
}
const codecReplaces = new Map([
    ["vp9", "vp09.00.10.08"]
]);
function classifyFormat(f) {
    if (isNullOrUndefined(f.vcodec)
        || f.vcodec === "none") {
        return "audio";
    }
    return "video";
}
export function combineContentTypeAndCodec(content_type, codec) {
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
    constructor(fetcher, makeProxyURL) {
        this.fetcher = fetcher;
        this.makeProxyURL = makeProxyURL;
    }
    makeVideoRecord(f) {
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
    makeAudioRecord(f) {
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
    makeImageRecord(f) {
        const { content_type, url, width, height } = f;
        return {
            contentType: content_type,
            url: this.makeProxyURL(url).href,
            width,
            height,
            resolution: width * height
        };
    }
    async loadData(pageURLOrMetadata, prog) {
        if (isNullOrUndefined(pageURLOrMetadata)) {
            throw new Error("must provide a YouTube URL or a YTMetadata object");
        }
        if (isDefined(prog)) {
            prog.start();
        }
        let metadata = null;
        if (isString(pageURLOrMetadata)) {
            metadata = await this.fetcher
                .get(pageURLOrMetadata)
                .progress(prog)
                .object()
                .then(unwrapResponse);
        }
        else {
            if (isDefined(prog)) {
                prog.end(pageURLOrMetadata.title);
            }
            metadata = pageURLOrMetadata;
        }
        let startTime = 0;
        if (isDefined(metadata.original_url)) {
            const url = new URL(metadata.original_url);
            if (isYouTube(url) && url.searchParams.has("t")) {
                startTime = parseFloat(url.searchParams.get("t"));
            }
        }
        const formats = new PriorityList((await Promise.all(metadata.formats))
            .map((f) => [classifyFormat(f), f]));
        const title = metadata.title;
        const thumbnails = metadata.thumbnails || [];
        const thumbnail = metadata.thumbnail && this.makeImageRecord(arrayScan(thumbnails, (t) => t.url === metadata.thumbnail));
        const videos = formats.get("video").map((f) => this.makeVideoRecord(f));
        const audios = formats.get("audio").map((f) => this.makeAudioRecord(f));
        const data = {
            title,
            thumbnail,
            videos,
            audios,
            startTime
        };
        return data;
    }
}
//# sourceMappingURL=YouTubeProxy.js.map