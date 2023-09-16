import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { FullVideoRecord } from "./data";
import { YTMetadata } from "./yt-dlp";
export declare function isYouTube(url: URL): boolean;
export declare function combineContentTypeAndCodec(content_type: string, codec: string): string;
export declare class YouTubeProxy {
    protected fetcher: IFetcher;
    protected readonly makeProxyURL: (path: string | URL) => URL;
    constructor(fetcher: IFetcher, makeProxyURL: (path: string | URL) => URL);
    private makeVideoRecord;
    private makeAudioRecord;
    private makeImageRecord;
    loadData(pageURLOrMetadata: string | YTMetadata, prog?: IProgress): Promise<FullVideoRecord>;
}
//# sourceMappingURL=YouTubeProxy.d.ts.map