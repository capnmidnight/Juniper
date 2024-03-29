import { arrayClear } from "@juniper-lib/collections/dist/arrays";
import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
import type { IProgress } from "./IProgress";

export class BaseProgress<T extends TypedEventMap<string> = TypedEventMap<string>>
    extends TypedEventTarget<T>
    implements IProgress {
    private readonly attached = new Array<IProgress>();
    private soFar: number = null;
    private total: number = null;
    private msg: string = null;
    private est: number = null;

    protected get p() {
        return this.total > 0
            ? this.soFar / this.total
            : 0;
    }

    report(soFar: number, total: number, msg?: string, est?: number): void {
        this.soFar = soFar;
        this.total = total;
        this.msg = msg;
        this.est = est;
        for (const attach of this.attached) {
            attach.report(soFar, total, msg, est);
        }
    }

    attach(prog: IProgress): void {
        this.attached.push(prog);
        prog.report(this.soFar, this.total, this.msg, this.est);
    }

    clear() {
        this.report(0, 0);
        this._clear();
    }

    start(msg?: string) {
        this.report(0, 1, msg || "starting");
    }

    end(msg?: string) {
        this.report(1, 1, msg || "done");
        this._clear();
    }

    private _clear() {
        this.soFar = null;
        this.total = null;
        this.msg = null;
        this.est = null;
        arrayClear(this.attached);
    }
}