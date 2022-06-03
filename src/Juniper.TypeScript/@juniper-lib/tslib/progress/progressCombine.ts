import { isDefined } from "../";
import { BaseProgress } from "./BaseProgress";
import { IProgress } from "./IProgress";

class CombinedProgressCallback extends BaseProgress {
    constructor(private readonly onProgs: IProgress[]) {
        super();
    }

    override report(soFar: number, total: number, message?: string, est?: number) {
        super.report(soFar, total, message, est);

        for (const onProg of this.onProgs) {
            onProg.report(soFar, total, message);
        }
    }
}

export function progressCombine(...onProgs: IProgress[]): IProgress {
    onProgs = onProgs.filter(isDefined);
    return new CombinedProgressCallback(onProgs);
}
