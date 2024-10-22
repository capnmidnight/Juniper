import { BaseTimer } from "./BaseTimer";
export declare class AsyncCallbackTimer extends BaseTimer<() => Promise<void>> {
    #private;
    get isRunning(): boolean;
    constructor();
    start(): void;
    stop(): void;
}
//# sourceMappingURL=AsyncCallbackTimer.d.ts.map