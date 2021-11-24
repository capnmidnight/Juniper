import type { IProgress } from "../";
import { BaseParentProgressCallback } from "../";
export declare function progressPopper(progress: IProgress): PoppableParentProgressCallback;
export declare class PoppableParentProgressCallback extends BaseParentProgressCallback {
    pop(weight?: number): IProgress;
}
