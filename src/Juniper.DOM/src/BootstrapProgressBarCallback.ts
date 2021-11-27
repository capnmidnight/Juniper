import { BaseProgress } from "juniper-tslib";
import { elementSetText } from "./tags";

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
            const percent = (100 * soFar / total).toFixed(1);
            this.progressBar.style.width = `${percent}%`;
            this.progressBar.setAttribute("aria-valuenow", percent);
        }

        if (this.showMessage) {
            elementSetText(this.progressBar, message || "");
        }
    }
}

