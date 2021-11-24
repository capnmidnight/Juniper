import { BaseParentProgressCallback } from "../";
export function progressSplitWeighted(onProgress, subProgressWeights) {
    const prog = new WeightedParentProgressCallback(subProgressWeights, onProgress);
    return prog.subProgressCallbacks;
}
export function progressSplit(onProgress, taskCount) {
    const subProgressWeights = new Array(taskCount);
    for (let i = 0; i < taskCount; ++i) {
        subProgressWeights[i] = 1;
    }
    return progressSplitWeighted(onProgress, subProgressWeights);
}
class WeightedParentProgressCallback extends BaseParentProgressCallback {
    constructor(subProgressWeights, prog) {
        super(prog);
        for (const weight of subProgressWeights) {
            this.addSubProgress(weight);
        }
    }
}
