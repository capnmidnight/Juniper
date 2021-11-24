import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { IProgress } from "./IProgress";

export function progressSplitWeighted(onProgress: IProgress | undefined, subProgressWeights: number[]) {
    const prog = new WeightedParentProgressCallback(subProgressWeights, onProgress);
    return prog.subProgressCallbacks;
}


export function progressSplit(onProgress: IProgress, taskCount: number) {
    const subProgressWeights = new Array<number>(taskCount);
    for (let i = 0; i < taskCount; ++i) {
        subProgressWeights[i] = 1;
    }

    return progressSplitWeighted(onProgress, subProgressWeights);
}

class WeightedParentProgressCallback extends BaseParentProgressCallback {

    constructor(subProgressWeights: number[], prog: IProgress) {
        super(prog);

        for (const weight of subProgressWeights) {
            this.addSubProgress(weight);
        }
    }
}
