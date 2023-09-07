import { ChildProgressCallback } from "./ChildProgressCallback";
export class BaseParentProgressCallback {
    constructor(prog) {
        this.prog = prog;
        this.weightTotal = 0;
        this.subProgressCallbacks = new Array();
        this.subProgressWeights = new Array();
        this.subProgressValues = new Array();
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
}
//# sourceMappingURL=BaseParentProgressCallback.js.map