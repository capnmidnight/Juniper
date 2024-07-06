import type { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { BaseProgress } from "./BaseProgress";

export class ChildProgressCallback extends BaseProgress {

    readonly #i: number;
    readonly #prog: BaseParentProgressCallback;

    constructor(i: number, prog: BaseParentProgressCallback) {
        super();
        this.#i = i;
        this.#prog = prog;
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);
        this.#prog.update(this.#i, soFar, total, msg);
    }
}
