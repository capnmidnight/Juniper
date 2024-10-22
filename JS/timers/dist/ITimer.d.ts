import { TypedEvent } from "@juniper-lib/events";
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
export type TickHandler = (evt: TimerTickEvent) => void;
export interface ITimer {
    isRunning: boolean;
    start(): void;
    stop(): void;
    restart(): void;
    addTickHandler(onTick: TickHandler): void;
    removeTickHandler(onTick: TickHandler): void;
}
//# sourceMappingURL=ITimer.d.ts.map