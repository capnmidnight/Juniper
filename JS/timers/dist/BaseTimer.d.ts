import { ITimer, TickHandler, TimerTickEvent } from "./ITimer";
export declare abstract class BaseTimer<CallbackT extends Function> implements ITimer {
    protected onTick: CallbackT;
    protected lt: number;
    protected tickEvt: TimerTickEvent;
    protected tickHandlers: TickHandler[];
    addTickHandler(onTick: TickHandler): void;
    removeTickHandler(onTick: TickHandler): void;
    restart(): void;
    abstract get isRunning(): boolean;
    abstract start(): void;
    stop(): void;
}
//# sourceMappingURL=BaseTimer.d.ts.map