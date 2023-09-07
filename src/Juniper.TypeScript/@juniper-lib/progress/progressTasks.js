import { progressSplitWeighted } from "./progressSplit";
export async function progressTasksWeighted(prog, taskDefs) {
    const weights = new Array(taskDefs.length);
    const callbacks = new Array(taskDefs.length);
    for (let i = 0; i < taskDefs.length; ++i) {
        const taskDef = taskDefs[i];
        weights[i] = taskDef[0];
        callbacks[i] = taskDef[1];
    }
    const progs = progressSplitWeighted(prog, weights);
    const tasks = new Array(taskDefs.length);
    for (let i = 0; i < taskDefs.length; ++i) {
        tasks[i] = callbacks[i](progs[i]);
    }
    return await Promise.all(tasks);
}
export function progressTasks(prog, ...subTaskDef) {
    const taskDefs = subTaskDef.map((t) => [1, t]);
    return progressTasksWeighted(prog, taskDefs);
}
//# sourceMappingURL=progressTasks.js.map