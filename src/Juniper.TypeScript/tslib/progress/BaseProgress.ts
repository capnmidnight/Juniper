import { arrayClear } from "../collections/arrayClear";
import type { IProgress } from "./IProgress";

export abstract class BaseProgress implements IProgress {
    private readonly attached = new Array<IProgress>();
    private soFar: number = null;
    private total: number = null;
    private msg: string = null;
    private est: number = null;

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

    end() {
        this.report(1, 1, "done");
        this.soFar = null;
        this.total = null;
        this.msg = null;
        this.est = null;
        arrayClear(this.attached);
    }
}