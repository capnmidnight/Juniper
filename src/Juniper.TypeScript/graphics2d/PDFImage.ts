import { setContextSize } from "juniper-dom/canvas";
import { IFetcher } from "juniper-fetcher-base/IFetcher";
import { clamp, IProgress, singleton } from "juniper-tslib";
import pdfJS from "pdfjs-dist";
import { version as pdfjsVersion } from "pdfjs-dist/package.json";
import { GetViewportParameters } from "pdfjs-dist/types/src/display/api";
import { CanvasImage } from "./CanvasImage";

let onReady: () => void = null;
const pdfReady = singleton("Juniper:PdfReady", () =>
    new Promise<void>((resolve) =>
        onReady = singleton("Juniper:PdfReadySignal", () =>
            resolve)));

export class PDFImage extends CanvasImage {

    static async prepare(workerPath: string, fetcher: IFetcher, prog?: IProgress) {
        if (onReady) {

            console.info(`PDF.js v${pdfjsVersion}`);

            const { content: workerSrc } = await fetcher
                .get(workerPath)
                .query("v", pdfjsVersion)
                .progress(prog)
                .file();
            pdfJS.GlobalWorkerOptions.workerSrc = workerSrc;
            onReady();
            onReady = null;
        }

        await pdfReady;
    }

    public readonly ready: Promise<void>;
    private pdf: pdfJS.PDFDocumentProxy = null;
    private curPage: number = null;

    constructor(filePath: string, private readonly viewportParams: GetViewportParameters) {
        super(1, 1);

        this.ready = this.load(filePath);
    }

    private async load(filePath: string) {
        await pdfReady;

        const pdfTask = pdfJS.getDocument(filePath);
        this.pdf = await pdfTask.promise;

        if (this.pdf.numPages === 0) {
            throw new Error("No pages found in PDF");
        }
    }

    public async getPage(pageIndex: number) {
        await this.ready;

        pageIndex = clamp(pageIndex, 0, this.pdf.numPages - 1);
        if (pageIndex !== this.curPage) {
            this.curPage = pageIndex;

            const page = await this.pdf.getPage(pageIndex + 1);
            const viewport = page.getViewport(this.viewportParams);

            setContextSize(this.g, viewport.width, viewport.height);
            const renderTask = page.render({
                canvasContext: this.g,
                viewport,
                intent: "print"
            });
            await renderTask.promise;
            this.dispatchEvent(this.redrawnEvt);
        }
    }

    get numPages(): number {
        if (this.pdf) {
            return this.pdf.numPages;
        }

        return null;
    }

    protected onRedraw(): boolean {
        return false;
    }
}
