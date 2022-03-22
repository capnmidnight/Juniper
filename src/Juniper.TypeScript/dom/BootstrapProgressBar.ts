import { BaseProgress, IProgress } from "juniper-tslib";
import { ariaValueMax, ariaValueMin, ariaValueNow, className, role } from "./attrs";
import { styles, width } from "./css";
import { Div, elementSetText, ErsatzElement } from "./tags";

export class BootstrapProgressBarElement
    implements ErsatzElement, IProgress {

    public readonly element: HTMLElement;
    private readonly progress: IProgress;

    constructor(outerClassName: string = "controls progress") {
        const inner = Div(
            className("progress-bar"),
            role("progressbar"),
            ariaValueNow(0),
            ariaValueMin(0),
            ariaValueMax(0),
            styles(
                width(0)
            )
        );

        this.element = Div(
            className(outerClassName),
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
            const percent = this.p.toFixed(1);
            this.progressBar.style.width = `${percent}%`;
            this.progressBar.setAttribute("aria-valuenow", percent);
        }

        if (this.showMessage) {
            elementSetText(this.progressBar, message || "");
        }
    }
}

export function BootstrapProgressBar(outerClassName: string = "controls progress") {
    return new BootstrapProgressBarElement(outerClassName);
}