import { BaseManagedTimer } from "./BaseManagedTimer";
export declare abstract class BaseVariableTimer<TimerT> extends BaseManagedTimer<TimerT> {
    #private;
    get targetFPS(): number;
    set targetFPS(v: number);
    constructor(targetFrameRate: number);
    protected get targetFrameTime(): number;
}
//# sourceMappingURL=BaseVariableTimer.d.ts.map