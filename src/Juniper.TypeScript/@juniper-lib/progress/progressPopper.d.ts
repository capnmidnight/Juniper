import { BaseParentProgressCallback } from "./BaseParentProgressCallback";
import { IProgress } from "./IProgress";
export declare function progressPopper(progress: IProgress): PoppableParentProgressCallback;
export declare class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight?: number): IProgress;
}
//# sourceMappingURL=progressPopper.d.ts.map