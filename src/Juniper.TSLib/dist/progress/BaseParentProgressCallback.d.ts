import type { IProgress } from "./IProgress";
export declare class BaseParentProgressCallback {
    private readonly prog;
    private weightTotal;
    private readonly start;
    readonly subProgressCallbacks: IProgress[];
    private readonly subProgressWeights;
    private readonly subProgressValues;
    constructor(prog: IProgress);
    protected addSubProgress(weight?: number): IProgress;
    update(i: number, subSoFar: number, subTotal: number, msg?: string): void;
}
