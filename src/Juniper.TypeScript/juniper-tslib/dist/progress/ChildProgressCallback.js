import { BaseProgress } from "./BaseProgress";
export class ChildProgressCallback extends BaseProgress {
    i;
    prog;
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
