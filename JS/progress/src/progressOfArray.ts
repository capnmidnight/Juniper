import { IProgress } from "./IProgress";
import { progressSplitWeighted } from "./progressSplit";

export function progressOfArray<T, U>(prog: IProgress, items: T[], callback: (val: T, prog: IProgress, i?: number) => Promise<U>): Promise<U[]> {
    const weights = items.map(() => 1);
    const progs = progressSplitWeighted(prog, weights);
    return Promise.all(items.map(async (item, i) => {
        const prog = progs[i];
        prog.start();
        const value = await callback(item, prog, i);
        prog.end();
        return value;
    }));
}
