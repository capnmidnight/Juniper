import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { IProgress } from "./IProgress";

export function progressSplitWeighted(prog: IProgress, subProgressWeights: number[]) {
    const subProg = new WeightedParentProgressCallback(subProgressWeights, prog);
    return subProg.subProgressCallbacks;
}


export function progressSplit(prog: IProgress, taskCount: number) {
    const subProgressWeights = new Array<number>(taskCount);
    for (let i = 0; i < taskCount; ++i) {
        subProgressWeights[i] = 1;
    }

    return progressSplitWeighted(prog, subProgressWeights);
}

class WeightedParentProgressCallback extends BaseParentProgressCallback {

    constructor(subProgressWeights: number[], prog: IProgress) {
        super(prog);

        for (const weight of subProgressWeights) {
            this.addSubProgress(weight);
        }
    }
}
