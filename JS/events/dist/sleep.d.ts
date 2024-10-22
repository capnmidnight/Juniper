import { Task } from "./Task";
export declare class SleepTask extends Task {
    #private;
    constructor(milliseconds: number);
    start(): void;
    reset(): void;
}
export declare function sleep(milliseconds: number): SleepTask;
//# sourceMappingURL=sleep.d.ts.map