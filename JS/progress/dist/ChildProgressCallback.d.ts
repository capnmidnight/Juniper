import type { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { BaseProgress } from "./BaseProgress";
export declare class ChildProgressCallback extends BaseProgress {
    #private;
    constructor(i: number, prog: BaseParentProgressCallback);
    report(soFar: number, total: number, msg?: string, est?: number): void;
}
//# sourceMappingURL=ChildProgressCallback.d.ts.map