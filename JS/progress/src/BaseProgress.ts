import { arrayClear, isDefined } from "@juniper-lib/util";
import { TypedEvent, TypedEventMap, TypedEventTarget } from "@juniper-lib/events";
import type { IProgress } from "./IProgress";

export class ProgressEvent extends TypedEvent<"progress"> {
    constructor(public readonly progress: BaseProgress) {
        super("progress");
    }
}

function estimate(start: number, end: number, p: number) {
    const delta = end - start;
    return start + delta / p;
}

function formatSeconds(totalSeconds: number) {
    const seconds = totalSeconds % 60;
    let est = seconds + "s";
    totalSeconds -= seconds;
    if(totalSeconds > 0){
        let totalMinutes = Math.round(totalSeconds / 60);
        const minutes = totalMinutes % 60;
        totalMinutes -= minutes;
        est = minutes + "m " + est;

        if(totalMinutes > 0) {
            const totalHours = Math.round(totalMinutes / 60);
            est = totalHours + "h " + est;
        }
    }

    return est;
}

export class BaseProgress<T extends TypedEventMap<string> = TypedEventMap<string>>
    extends TypedEventTarget<T & {
        "progress": ProgressEvent;
    }>
    implements IProgress {

    readonly #attached = new Array<IProgress>();

    #soFar: number = null;
    #total: number = null;

    #msg: string = null;
    get message() { return this.#msg; }

    #start: number = null;
    #est: number = null;
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

    readonly #evt: ProgressEvent;

    constructor() {
        super();
        this.#evt = new ProgressEvent(this);
    }

    get p() {
        return this.#total > 0
            ? this.#soFar / this.#total
            : 0;
    }

    report(soFar: number, total: number, msg?: string, est?: number): void {
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

    attach(prog: IProgress): void {
        this.#attached.push(prog);
        prog.report(this.#soFar, this.#total, this.#msg, this.#est);
    }

    clear() {
        this.report(0, 0);
        this.#clear();
    }

    start(msg?: string) {
        this.#start = performance.now();
        this.report(0, 1, msg || "starting");
    }

    end(msg?: string) {
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