import { setContextSize } from "@juniper-lib/dom/dist/canvas";
import { CancelToken } from "@juniper-lib/events/dist/CancelToken";
import { Task } from "@juniper-lib/events/dist/Task";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import { URLBuilder } from "@juniper-lib/tslib/dist/URLBuilder";
import { clamp } from "@juniper-lib/tslib/dist/math";
import { singleton } from "@juniper-lib/tslib/dist/singleton";
import * as pdfJS from "pdfjs-dist";
import { version as pdfjsVersion } from "pdfjs-dist/package.json";
import { CanvasImage } from "./CanvasImage";
const pdfReady = singleton("Juniper:PdfReady", () => new Task(false));
export class PDFImage extends CanvasImage {
    static async prepare(workerPath, fetcher, debug, tokenOrProg, prog) {
        let token = null;
        if (tokenOrProg instanceof CancelToken) {
            token = tokenOrProg;
        }
        else {
            prog = tokenOrProg;
        }
        token = token || new CancelToken();
        if (!pdfReady.started) {
            pdfReady.start();
            console.info(`PDF.js v${pdfjsVersion}`);
            const uri = new URLBuilder(workerPath, location.href);
            uri.query("v", pdfjsVersion);
            workerPath = uri.toString();
            pdfJS.GlobalWorkerOptions.workerSrc = await fetcher
                .get(workerPath)
                .useCache(!debug)
                .progress(prog)
                .file()
                .then(unwrapResponse);
            token.check();
            pdfReady.resolve();
        }
        await pdfReady;
        token.check();
    }
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
    constructor(filePath, viewportParams) {
        super(1, 1);
        this.viewportParams = viewportParams;
        this.pdf = null;
        this._curPageIndex = null;
        this.ready = this.load(filePath);
    }
    async load(filePath) {
        await pdfReady;
        const pdfTask = pdfJS.getDocument(filePath);
        this.pdf = await pdfTask.promise;
        if (this.pdf.numPages === 0) {
            throw new Error("No pages found in PDF");
        }
    }
    async getPage(pageIndex) {
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
    get numPages() {
        if (this.pdf) {
            return this.pdf.numPages;
        }
        return null;
    }
    onRedraw() {
        return false;
    }
}
//# sourceMappingURL=PDFImage.js.map