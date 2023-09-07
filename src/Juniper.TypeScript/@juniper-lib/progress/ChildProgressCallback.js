import { BaseProgress } from "./BaseProgress";
export class ChildProgressCallback extends BaseProgress {
    constructor(i, prog) {
        super();
        this.i = i;
        this.prog = prog;
    }
    report(soFar, total, msg, est) {
        super.report(soFar, total, msg, est);
        this.prog.update(this.i, soFar, total, msg);
    }
}
//# sourceMappingURL=ChildProgressCallback.js.map