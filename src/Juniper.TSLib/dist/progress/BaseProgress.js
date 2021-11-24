import { arrayClear } from "../";
export class BaseProgress {
    attached = new Array();
    soFar = null;
    total = null;
    msg = null;
    est = null;
    report(soFar, total, msg, est) {
        this.soFar = soFar;
        this.total = total;
        this.msg = msg;
        this.est = est;
        for (const attach of this.attached) {
            attach.report(soFar, total, msg, est);
        }
    }
    attach(prog) {
        this.attached.push(prog);
        prog.report(this.soFar, this.total, this.msg, this.est);
    }
    end() {
        this.report(1, 1, "done");
        this.soFar = null;
        this.total = null;
        this.msg = null;
        this.est = null;
        arrayClear(this.attached);
    }
}
