import { progressSplitWeighted } from "../";
export async function progressOfArray(onProgress, items, callback) {
    const weights = items.map(() => 1);
    const progs = progressSplitWeighted(onProgress, weights);
    const tasks = items.map((item, i) => callback(item, progs[i], i));
    return await Promise.all(tasks);
}
