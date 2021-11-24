import { BaseProgress } from "../";
export class BaseParentProgressCallback {
    prog;
    weightTotal = 0;
    start;
    subProgressCallbacks = new Array();
    subProgressWeights = new Array();
    subProgressValues = new Array();
    constructor(prog) {
        this.prog = prog;
        this.start = performance.now();
        for (let i = 0; i < this.subProgressWeights.length; ++i) {
            this.subProgressValues[i] = 0;
            this.subProgressCallbacks[i] = new ChildProgressCallback(i, this);
        }
    }
    addSubProgress(weight) {
        weight = weight || 1;
        this.weightTotal += weight;
        this.subProgressWeights.push(weight);
        this.subProgressValues.push(0);
        const child = new ChildProgressCallback(this.subProgressCallbacks.length, this);
        this.subProgressCallbacks.push(child);
        return child;
    }
    update(i, subSoFar, subTotal, msg) {
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
    }
    ;
}
class ChildProgressCallback extends BaseProgress {
    i;
    prog;
    constructor(i, prog) {
        super();
        this.i = i;
        this.prog = prog;
    }
    report(soFar, total, msg, est) {
        super.report(soFar, total, msg, est);
        this.prog.update(this.i, soFar, total, msg);
    }
}
