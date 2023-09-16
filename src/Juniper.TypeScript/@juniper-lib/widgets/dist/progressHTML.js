import { BaseProgress } from "@juniper-lib/progress/dist/BaseProgress";
export function progressHTML(prog) {
    return new HTMLProgressBarCallback(prog);
}
class HTMLProgressBarCallback extends BaseProgress {
    constructor(prog) {
        super();
        this.prog = prog;
    }
    report(soFar, total, message, estimate) {
        super.report(soFar, total, message, estimate);
        this.prog.max = total;
        this.prog.value = soFar;
    }
}
//# sourceMappingURL=progressHTML.js.map