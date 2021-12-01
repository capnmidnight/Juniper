import { ITimer, TimerTickEvent } from "./ITimer";
export declare abstract class BaseTimer<TimerT> implements ITimer {
    protected _timer: TimerT;
    protected _onTick: (t: number, frame?: XRFrame) => void;
    protected _targetFrameTime: number;
    private _targetFPS;
    private lt;
    private tickHandlers;
    constructor(targetFrameRate: number);
    addTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    private tick;
    restart(): void;
    get isRunning(): boolean;
    abstract start(): void;
    stop(): void;
    get targetFPS(): number;
    set targetFPS(fps: number);
    get targetFrameTime(): number;
}
