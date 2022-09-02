import { TypedEvent } from "../events/EventBase";
import { lerp } from "../math";

export abstract class BaseTimerTickEvent extends TypedEvent<"update"> {
    t = 0;
    dt = 0;
    sdt = 0;
    fps: number = 0;

    constructor() {
        super("update");
    }

    set(t: number, dt: number) {
        this.t = t;
        this.dt = dt;
        this.sdt = lerp(this.sdt, dt, 0.01);
        if (dt > 0) {
            this.fps = 1000 / dt;
        }
    }
}

export class TimerTickEvent extends BaseTimerTickEvent {
    constructor() {
        super();
        Object.seal(this);
    }
}

export interface ITimer {
    isRunning: boolean;
    start(): void;
    stop(): void;
    restart(): void;
    addTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void;
}
