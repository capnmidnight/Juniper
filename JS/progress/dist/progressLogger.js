import { BaseProgress } from "./BaseProgress";
class ConsoleProgressCallback extends BaseProgress {
    constructor(name, prog) {
        super();
        this.name = name;
        this.prog = prog;
    }
    report(soFar, total, msg, est) {
        super.report(soFar, total, msg, est);
        console.log(this.name, (100 * this.p).toFixed(1), msg);
        if (this.prog) {
            this.prog.report(soFar, total, msg, est);
        }
    }
}
export function progressLogger(name, prog) {
    return new ConsoleProgressCallback(name, prog);
}
//# sourceMappingURL=progressLogger.js.map