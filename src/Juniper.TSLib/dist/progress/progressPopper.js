import { BaseParentProgressCallback } from "../";
export function progressPopper(progress) {
    return new PoppableParentProgressCallback(progress);
}
export class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight) {
        return this.addSubProgress(weight);
    }
}
