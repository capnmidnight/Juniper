import { BaseProgress, IProgress } from "../";

export class BaseParentProgressCallback {
    private weightTotal = 0;
    private readonly start: number;

    readonly subProgressCallbacks = new Array<IProgress>();
    private readonly subProgressWeights = new Array<number>();
    private readonly subProgressValues = new Array<number>();

    constructor(private readonly prog: IProgress) {
        this.start = performance.now();

        for (let i = 0; i < this.subProgressWeights.length; ++i) {
            this.subProgressValues[i] = 0;
            this.subProgressCallbacks[i] = new ChildProgressCallback(i, this);
        }
    }

    protected addSubProgress(weight?: number): IProgress {
        weight = weight || 1;
        this.weightTotal += weight;
        this.subProgressWeights.push(weight);
        this.subProgressValues.push(0);
        const child = new ChildProgressCallback(this.subProgressCallbacks.length, this);
        this.subProgressCallbacks.push(child);
        return child;
    }


    update(i: number, subSoFar: number, subTotal: number, msg?: string) {
        if (this.prog) {
            this.subProgressValues[i] = subSoFar / subTotal;
            let soFar = 0;
            for (let j = 0; j < this.subProgressWeights.length; ++j) {
                soFar += this.subProgressValues[j] * this.subProgressWeights[j];
            }

            const end = performance.now();
            const delta = end - this.start;
            const est = this.start - end + delta * this.weightTotal / soFar;
            this.prog.report(soFar, this.weightTotal, msg, est);
        }
    };
}

class ChildProgressCallback extends BaseProgress {
    constructor(private readonly i: number, private readonly prog: BaseParentProgressCallback) {
        super();
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);
        this.prog.update(this.i, soFar, total, msg);
    }
}
