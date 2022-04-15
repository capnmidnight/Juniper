import { lerp } from "@juniper/math";

export class TimerTickEvent {
    t = 0;
    dt = 0;
    sdt = 0;
    fps: number = 0;

    frame: XRFrame = null;

    constructor() {
        Object.seal(this);
    }

    set(t: number, dt: number, frame: XRFrame) {
        this.t = t;
        this.dt = dt;
        this.sdt = lerp(this.sdt, dt, 0.01);
        this.frame = frame;
        if (dt > 0) {
            this.fps = 1000 / dt;
        }
    }
}

export interface ITimer {
    isRunning: boolean;
    targetFPS: number;
    start(): void;
    stop(): void;
    restart(): void;
    addTickHandler(onTick: (evt: TimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void;
}
