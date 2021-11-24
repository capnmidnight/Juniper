import type { IProgress } from "../";
export declare function progressSplitWeighted(onProgress: IProgress | undefined, subProgressWeights: number[]): IProgress[];
export declare function progressSplit(onProgress: IProgress, taskCount: number): IProgress[];
