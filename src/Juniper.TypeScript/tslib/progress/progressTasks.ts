import { IProgress } from "./IProgress";
import { progressSplitWeighted } from "./progressSplit";

export type subProgressCallback = (prog: IProgress) => Promise<any>;

export type TaskDef = [weight: number, task: subProgressCallback];

export async function progressTasksWeighted(prog: IProgress, taskDefs: TaskDef[]) {
    const weights = new Array<number>(taskDefs.length);
    const callbacks = new Array<subProgressCallback>(taskDefs.length);
    for (let i = 0; i < taskDefs.length; ++i) {
        const taskDef = taskDefs[i];
        weights[i] = taskDef[0];
        callbacks[i] = taskDef[1];
    }

    const progs = progressSplitWeighted(prog, weights);
    const tasks = new Array<Promise<any>>(taskDefs.length);
    for (let i = 0; i < taskDefs.length; ++i) {
        tasks[i] = callbacks[i](progs[i]);
    }

    return await Promise.all(tasks);
}

export function progressTasks(prog: IProgress, ...subTaskDef: subProgressCallback[]) {
    const taskDefs = subTaskDef.map<TaskDef>(t => [1, t]);
    return progressTasksWeighted(prog, taskDefs);
}