import { CancelToken } from "@juniper-lib/events/CancelToken";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { GetViewportParameters } from "pdfjs-dist/types/src/display/api";
import { CanvasImage } from "./CanvasImage";
export declare class PDFImage extends CanvasImage {
    private readonly viewportParams;
    static prepare(workerPath: string, fetcher: IFetcher, debug: boolean, tokenOrProg: CancelToken, prog?: IProgress): Promise<void>;
    static prepare(workerPath: string, fetcher: IFetcher, debug: boolean, prog?: IProgress): Promise<void>;
    readonly ready: Promise<void>;
    private pdf;
    private _curPageIndex;
    get curPageIndex(): number;
    get curPageNumber(): number;
    get canGoBack(): boolean;
    get canGoForward(): boolean;
    constructor(filePath: string, viewportParams: GetViewportParameters);
    private load;
    getPage(pageIndex: number): Promise<void>;
    get numPages(): number;
    protected onRedraw(): boolean;
}
//# sourceMappingURL=PDFImage.d.ts.map