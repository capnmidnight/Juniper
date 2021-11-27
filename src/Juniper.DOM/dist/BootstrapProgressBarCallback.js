import { BaseProgress } from "juniper-tslib";
import { elementSetText } from "./tags";
export class BootstrapProgressBarCallback extends BaseProgress {
    progressBar;
    showMessage;
    constructor(progressBar, showMessage = true) {
        super();
        this.progressBar = progressBar;
        this.showMessage = showMessage;
    }
    report(soFar, total, message, estimate) {
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
