import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
export function progressSplitWeighted(prog, subProgressWeights) {
    const subProg = new WeightedParentProgressCallback(subProgressWeights, prog);
    return subProg.subProgressCallbacks;
}
export function progressSplit(prog, taskCount) {
    const subProgressWeights = new Array(taskCount);
    for (let i = 0; i < taskCount; ++i) {
        subProgressWeights[i] = 1;
    }
    return progressSplitWeighted(prog, subProgressWeights);
}
class WeightedParentProgressCallback extends BaseParentProgressCallback {
    constructor(subProgressWeights, prog) {
        super(prog);
        for (const weight of subProgressWeights) {
            this.addSubProgress(weight);
        }
    }
}
//# sourceMappingURL=progressSplit.js.map