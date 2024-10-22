import { BaseTimer } from "./BaseTimer";
export declare abstract class BaseManagedTimer<TimerT> extends BaseTimer<(t: number) => void> {
    protected timer: TimerT;
    get isRunning(): boolean;
    constructor();
    stop(): void;
}
//# sourceMappingURL=BaseManagedTimer.d.ts.map