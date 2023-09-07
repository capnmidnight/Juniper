import { AriaValueMax, AriaValueMin, AriaValueNow, ClassList, Role } from "@juniper-lib/dom/attrs";
import { width } from "@juniper-lib/dom/css";
import { Div, elementSetText } from "@juniper-lib/dom/tags";
import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
export class BootstrapProgressBarElement {
    constructor(outerClassName = "controls progress") {
        const inner = Div(ClassList("progress-bar"), Role("progressbar"), AriaValueNow(0), AriaValueMin(0), AriaValueMax(1), width(0));
        this.element = Div(ClassList(outerClassName), inner);
        this.progress = new BootstrapProgressBarCallback(inner);
    }
    clear() {
        this.progress.clear();
    }
    report(soFar, total, message, est) {
        this.progress.report(soFar, total, message, est);
    }
    attach(prog) {
        this.progress.attach(prog);
    }
    start(msg) {
        this.progress.start(msg);
    }
    end(msg) {
        this.progress.end(msg);
    }
}
export class BootstrapProgressBarCallback extends BaseProgress {
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
//# sourceMappingURL=BootstrapProgressBar.js.map