import type { IProgress } from "./IProgress";
export declare class BaseParentProgressCallback {
    #private;
    readonly subProgressCallbacks: IProgress[];
    constructor(prog: IProgress);
    protected addSubProgress(weight?: number): IProgress;
    update(i: number, subSoFar: number, subTotal: number, msg?: string): void;
}
//# sourceMappingURL=BaseParentProgressCallback.d.ts.map