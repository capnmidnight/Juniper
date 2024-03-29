import { AriaValueMax, AriaValueMin, AriaValueNow, ClassList, Role } from "@juniper-lib/dom/dist/attrs";
import { width } from "@juniper-lib/dom/dist/css";
import { Div, ErsatzElement, elementSetText } from "@juniper-lib/dom/dist/tags";
import { BaseProgress } from "@juniper-lib/progress/dist/BaseProgress";
import { IProgress } from "@juniper-lib/progress/dist/IProgress";

export class BootstrapProgressBarElement implements ErsatzElement, IProgress {

    public readonly element: HTMLElement;
    private readonly progress: IProgress;

    constructor(outerClassName = "controls progress") {
        const inner = Div(
            ClassList("progress-bar"),
            Role("progressbar"),
            AriaValueNow(0),
            AriaValueMin(0),
            AriaValueMax(1),
            width(0)
        );

        this.element = Div(
            ClassList(outerClassName),
            inner
        );

        this.progress = new BootstrapProgressBarCallback(inner);
    }

    clear() {
        this.progress.clear();
    }

    report(soFar: number, total: number, message?: string, est?: number): void {
        this.progress.report(soFar, total, message, est);
    }

    attach(prog: IProgress): void {
        this.progress.attach(prog);
    }

    start(msg?: string) {
        this.progress.start(msg);
    }

    end(msg?: string): void {
        this.progress.end(msg);
    }

}

export class BootstrapProgressBarCallback extends BaseProgress {
    constructor(private readonly progressBar: HTMLElement, private readonly showMessage = true) {
        super();
    }

    override report(soFar: number, total: number, message?: string, estimate?: number) {
        super.report(soFar, total, message, estimate);

        if (soFar === 0) {
            this.progressBar.style.width = "0";
            this.progressBar.setAttribute("aria-valuenow", "0");
        }
        else {
            const percent = (100 * this.p).toFixed(1);
            this.progressBar.style.width = `${percent}%`;
            this.progressBar.setAttribute("aria-valuenow", percent);
        }

        if (this.showMessage) {
            elementSetText(this.progressBar, message || "");
        }
    }
}

export function BootstrapProgressBar(outerClassName = "controls progress") {
    return new BootstrapProgressBarElement(outerClassName);
}