import { arrayClear, isDefined } from "@juniper-lib/util";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
export class ProgressEvent extends TypedEvent {
    constructor(progress) {
        super("progress");
        this.progress = progress;
    }
}
function estimate(start, end, p) {
    const delta = end - start;
    return start + delta / p;
}
function formatSeconds(totalSeconds) {
    const seconds = totalSeconds % 60;
    let est = seconds + "s";
    totalSeconds -= seconds;
    if (totalSeconds > 0) {
        let totalMinutes = Math.round(totalSeconds / 60);
        const minutes = totalMinutes % 60;
        totalMinutes -= minutes;
        est = minutes + "m " + est;
        if (totalMinutes > 0) {
            const totalHours = Math.round(totalMinutes / 60);
            est = totalHours + "h " + est;
        }
    }
    return est;
}
export class BaseProgress extends TypedEventTarget {
    #attached = new Array();
    #soFar = null;
    #total = null;
    #msg = null;
    get message() { return this.#msg; }
    #start = null;
    #est = null;
    get estimate() { return this.#est; }
    get basicMessage() {
        if (Number.isFinite(this.#est) && Number.isFinite(this.#start)) {
            const now = performance.now();
            const elapsed = formatSeconds(Math.round((now - this.#start) / 1000));
            const remain = formatSeconds(Math.round((this.#est - now) / 1000));
            return `(${elapsed} elapsed, est. ${remain} remaining) ${this.#msg ?? ""}`.trim();
        }
        else {
            return this.#msg ?? "";
        }
    }
    #evt;
    constructor() {
        super();
        this.#evt = new ProgressEvent(this);
    }
    get p() {
        return this.#total > 0
            ? this.#soFar / this.#total
            : 0;
    }
    report(soFar, total, msg, est) {
        this.#soFar = soFar;
        this.#total = total;
        this.#msg = msg;
        if (isDefined(this.#start)) {
            this.#est = est || estimate(this.#start, performance.now(), this.p);
        }
        else {
            this.#est = null;
        }
        for (const attach of this.#attached) {
            attach.report(this.#soFar, this.#total, this.#msg, this.#est);
        }
        this.dispatchEvent(this.#evt);
    }
    attach(prog) {
        this.#attached.push(prog);
        prog.report(this.#soFar, this.#total, this.#msg, this.#est);
    }
    clear() {
        this.report(0, 0);
        this.#clear();
    }
    start(msg) {
        this.#start = performance.now();
        this.report(0, 1, msg || "starting");
    }
    end(msg) {
        this.report(1, 1, msg || "done");
        this.#clear();
    }
    #clear() {
        this.#soFar = null;
        this.#total = null;
        this.#msg = null;
        this.#start = null;
        this.#est = null;
        arrayClear(this.#attached);
    }
}
//# sourceMappingURL=BaseProgress.js.map