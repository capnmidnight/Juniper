import { ITimer, TimerTickEvent } from "./ITimer";
export declare abstract class BaseTimer<TimerT> implements ITimer {
    protected timer: TimerT;
    protected onTick: (t: number) => void;
    private lt;
    private tickHandlers;
    constructor(targetFrameRate?: number);
    private _targetFPS;
    get targetFPS(): number;
    set targetFPS(v: number);
    addTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    private tick;
    restart(): void;
    get isRunning(): boolean;
    abstract start(): void;
    stop(): void;
    protected get targetFrameTime(): number;
}
//# sourceMappingURL=BaseTimer.d.ts.map