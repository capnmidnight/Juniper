import { isDefined } from "@juniper-lib/util";
import { BaseProgress } from "./BaseProgress";
class CombinedProgressCallback extends BaseProgress {
    #onProgs;
    constructor(onProgs) {
        super();
        this.#onProgs = onProgs;
    }
    report(soFar, total, message, est) {
        super.report(soFar, total, message, est);
        for (const onProg of this.#onProgs) {
            onProg.report(soFar, total, message);
        }
    }
}
export function progressCombine(...onProgs) {
    onProgs = onProgs.filter(isDefined);
    return new CombinedProgressCallback(onProgs);
}
//# sourceMappingURL=progressCombine.js.map