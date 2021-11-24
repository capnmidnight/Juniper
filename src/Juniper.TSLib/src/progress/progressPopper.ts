import type { IProgress } from "../";
import { BaseParentProgressCallback } from "../";

export function progressPopper(progress: IProgress) {
    return new PoppableParentProgressCallback(progress);
}

export class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight?: number) {
        return this.addSubProgress(weight);
    }
}
