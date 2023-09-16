import { progressSplitWeighted } from "./progressSplit";
export function progressOfArray(prog, items, callback) {
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
//# sourceMappingURL=progressOfArray.js.map