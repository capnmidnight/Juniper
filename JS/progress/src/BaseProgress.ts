import { EventReplacer, arrayClear, eventHandler, replaceEventHandler } from "@juniper-lib/util";
import { TypedEventTarget, TypedEventMap, TypedEvent } from "@juniper-lib/events";
import type { IProgress } from "./IProgress";

export class ProgressEvent extends TypedEvent<"progress"> {
    constructor(public readonly progress: IProgress) {
        super("progress");
    }
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
    #est: number = null;

    readonly #evt: ProgressEvent;

    #onprogress: eventHandler<ProgressEvent> = null;
    get onprogress() { return this.#onprogress; }
    set onprogress(v) { this.#onprogress = this.#replaceEventHandler("progress", this.#onprogress, v); }
    
    #replaceEventHandler: EventReplacer;

    constructor() {
        super();
        this.#evt = new ProgressEvent(this);
        this.#replaceEventHandler = replaceEventHandler;
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
        this.#est = est;
        for (const attach of this.#attached) {
            attach.report(soFar, total, msg, est);
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
        this.#est = null;
        arrayClear(this.#attached);
    }
}