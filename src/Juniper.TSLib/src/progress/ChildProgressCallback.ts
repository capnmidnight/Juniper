import type { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { BaseProgress } from "./BaseProgress";

export class ChildProgressCallback extends BaseProgress {
    constructor(private readonly i: number, private readonly prog: BaseParentProgressCallback) {
        super();
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);
        this.prog.update(this.i, soFar, total, msg);
    }
}
