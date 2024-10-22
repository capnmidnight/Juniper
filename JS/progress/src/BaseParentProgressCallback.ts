import { ChildProgressCallback } from "./ChildProgressCallback";
import type { IProgress } from "./IProgress";

export class BaseParentProgressCallback {
    #weightTotal = 0;
    readonly #start: number;

    readonly subProgressCallbacks = new Array<IProgress>();
    readonly #subProgressWeights = new Array<number>();
    readonly #subProgressValues = new Array<number>();
    readonly #prog: IProgress;

    constructor(prog: IProgress) {
        this.#prog = prog;
        this.#start = performance.now();

        for (let i = 0; i < this.#subProgressWeights.length; ++i) {
            this.#subProgressValues[i] = 0;
            this.subProgressCallbacks[i] = new ChildProgressCallback(i, this);
        }
    }

    protected addSubProgress(weight?: number): IProgress {
        weight = weight || 1;
        this.#weightTotal += weight;
        this.#subProgressWeights.push(weight);
        this.#subProgressValues.push(0);
        const child = new ChildProgressCallback(this.subProgressCallbacks.length, this);
        this.subProgressCallbacks.push(child);
        return child;
    }


    update(i: number, subSoFar: number, subTotal: number, msg?: string) {
        if (this.#prog) {
            this.#subProgressValues[i] = subSoFar / subTotal;
            let soFar = 0;
            for (let j = 0; j < this.#subProgressWeights.length; ++j) {
                soFar += this.#subProgressValues[j] * this.#subProgressWeights[j];
            }

            const end = performance.now();
            const delta = end - this.#start;
            const est = this.#start - end + delta * this.#weightTotal / soFar;
            this.#prog.report(soFar, this.#weightTotal, msg, est);
        }
    }
}
