import { IProgress } from "./IProgress";
import { progressSplitWeighted } from "./progressSplit";

export async function progressOfArray<T, U>(onProgress: IProgress | undefined, items: T[], callback: (val: T, prog: IProgress, i?: number) => Promise<U>) {
    const weights = items.map(() => 1);
    const progs = progressSplitWeighted(onProgress, weights);
    const tasks = items.map((item, i) => callback(item, progs[i], i));
    return await Promise.all(tasks);
}
