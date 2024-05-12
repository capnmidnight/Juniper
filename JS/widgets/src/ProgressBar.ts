import { BaseProgress, IProgress } from "@juniper-lib/progress";
import { AriaValueMax, AriaValueMin, AriaValueNow, ClassList, Clear, Div, ElementChild, HtmlRender, HtmlTag, Role, TitleAttr, perc, width } from "@juniper-lib/dom";


export class BootstrapProgressBarCallback extends BaseProgress {
    #progressBar: HTMLElement;
    #showMessage: boolean;

    constructor(progressBar: HTMLElement, showMessage: boolean = true) {
        super();
        this.#progressBar = progressBar;
        this.#showMessage = showMessage;
    }

    override report(soFar: number, total: number, message: string, estimate: number): void {
        super.report(soFar, total, message, estimate);

        const value = 100 * this.p;
        HtmlRender(this.#progressBar,
            AriaValueNow(value),
            width(perc(value))
        );

        if (this.#showMessage) {
            message = message || "";
            HtmlRender(
                this.#progressBar,
                Clear(),
                TitleAttr(message),
                message);
        }
    }
}

export class BootstrapProgressBarElement extends HTMLElement implements IProgress {

    #progress: BootstrapProgressBarCallback;

    constructor() {
        super();

        const inner = Div(
            ClassList("progress-bar"),
            Role("progressbar"),
            AriaValueNow(0),
            AriaValueMin(0),
            AriaValueMax(1),
            width(0)
        );

        HtmlRender(this,
            ClassList("progress"),
            inner
        );

        this.#progress = new BootstrapProgressBarCallback(inner);
    }
    
    get p(): number { return this.#progress.p; }

    clear(): void {
        this.#progress.clear();
    }


    report(soFar: number, total: number, message: string, est: number): void {
        this.#progress.report(soFar, total, message, est);
    }

    attach(prog: IProgress): void {
        this.#progress.attach(prog);
    }


    start(msg: string): void {
        this.#progress.start(msg);
    }

    end(msg: string): void {
        this.#progress.end(msg);
    }

}
customElements.define("progress-bar", BootstrapProgressBarElement);

export function ProgressBar(...sub: ElementChild[]): BootstrapProgressBarElement { return HtmlTag("progress-bar", ...sub); }
