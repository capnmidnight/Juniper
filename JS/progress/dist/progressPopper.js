import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
export function progressPopper(progress) {
    return new PoppableParentProgressCallback(progress);
}
export class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight) {
        return this.addSubProgress(weight);
    }
}
//# sourceMappingURL=progressPopper.js.map