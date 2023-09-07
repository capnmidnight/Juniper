import { arrayClear } from "@juniper-lib/collections/arrays";
import { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
export class BaseProgress extends TypedEventTarget {
    constructor() {
        super(...arguments);
        this.attached = new Array();
        this.soFar = null;
        this.total = null;
        this.msg = null;
        this.est = null;
    }
    get p() {
        return this.total > 0
            ? this.soFar / this.total
            : 0;
    }
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
    clear() {
        this.report(0, 0);
        this._clear();
    }
    start(msg) {
        this.report(0, 1, msg || "starting");
    }
    end(msg) {
        this.report(1, 1, msg || "done");
        this._clear();
    }
    _clear() {
        this.soFar = null;
        this.total = null;
        this.msg = null;
        this.est = null;
        arrayClear(this.attached);
    }
}
//# sourceMappingURL=BaseProgress.js.map