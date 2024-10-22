import { IProgress } from "./IProgress";
export type subProgressCallback = (prog: IProgress) => Promise<any>;
export type TaskDef = [weight: number, task: subProgressCallback];
export declare function progressTasksWeighted(prog: IProgress, taskDefs: TaskDef[]): Promise<any[]>;
export declare function progressTasks(prog: IProgress, ...subTaskDef: subProgressCallback[]): Promise<any[]>;
//# sourceMappingURL=progressTasks.d.ts.map