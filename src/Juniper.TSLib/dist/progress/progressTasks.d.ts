import type { IProgress } from "../";
export declare type subProgressCallback = (onProgress: IProgress) => Promise<any>;
export declare type TaskDef = [weight: number, task: subProgressCallback];
export declare function progressTasksWeighted(prog: IProgress, taskDefs: TaskDef[]): Promise<any[]>;
export declare function progressTasks(prog: IProgress, ...subTaskDef: subProgressCallback[]): Promise<any[]>;
