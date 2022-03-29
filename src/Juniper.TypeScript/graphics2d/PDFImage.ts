import { setContextSize } from "juniper-dom/canvas";
import { IFetcher } from "juniper-fetcher-base/IFetcher";
import { clamp, IProgress, singleton, Task } from "juniper-tslib";
import pdfJS from "pdfjs-dist";
import { version as pdfjsVersion } from "pdfjs-dist/package.json";
import { GetViewportParameters } from "pdfjs-dist/types/src/display/api";
import { CanvasImage } from "./CanvasImage";

const pdfReady = singleton("Juniper:PdfReady", () => new Task(false));

export class PDFImage extends CanvasImage {

    static async prepare(workerPath: string, fetcher: IFetcher, prog?: IProgress) {
        if (!pdfReady.started) {
            pdfReady.start();
            console.info(`PDF.js v${pdfjsVersion}`);

            const { content: workerSrc } = await fetcher
                .get(workerPath)
                .query("v", pdfjsVersion)
                .progress(prog)
                .file();

            pdfJS.GlobalWorkerOptions.workerSrc = workerSrc;
            pdfReady.resolve();
        }

        await pdfReady;
    }

    public readonly ready: Promise<void>;
    private pdf: pdfJS.PDFDocumentProxy = null;

    private _curPageIndex: number = null;
    get curPageIndex() {
        return this._curPageIndex;
    }

    get curPageNumber() {
        return this.curPageIndex + 1;
    }

    get canGoBack() {
        return this._curPageIndex > 0;
    }

    get canGoForward() {
        return this._curPageIndex < this.numPages - 1;
    }

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
        if (pageIndex !== this._curPageIndex) {
            this._curPageIndex = pageIndex;

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
