import { BaseTimer } from "./BaseTimer";
export declare class SetIntervalTimer extends BaseTimer<number> {
    constructor(targetFrameRate: number);
    start(): void;
    stop(): void;
    get targetFPS(): number;
    set targetFPS(fps: number);
}
//# sourceMappingURL=SetIntervalTimer.d.ts.map