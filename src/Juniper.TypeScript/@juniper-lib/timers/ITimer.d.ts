import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export declare abstract class BaseTimerTickEvent extends TypedEvent<"update"> {
    t: number;
    dt: number;
    sdt: number;
    fps: number;
    constructor();
    set(t: number, dt: number): void;
}
export declare class TimerTickEvent extends BaseTimerTickEvent {
    constructor();
}
export interface ITimer {
    isRunning: boolean;
    start(): void;
    stop(): void;
    restart(): void;
    addTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void;
}
//# sourceMappingURL=ITimer.d.ts.map