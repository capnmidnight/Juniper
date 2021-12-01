import { IProgress } from "./IProgress";
export declare function progressOfArray<T, U>(onProgress: IProgress | undefined, items: T[], callback: (val: T, prog: IProgress, i?: number) => Promise<U>): Promise<U[]>;
