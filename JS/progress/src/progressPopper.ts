import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { IProgress } from "./IProgress";

export function progressPopper(progress: IProgress) {
    return new PoppableParentProgressCallback(progress);
}

export class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight?: number) {
        return this.addSubProgress(weight);
    }
}
