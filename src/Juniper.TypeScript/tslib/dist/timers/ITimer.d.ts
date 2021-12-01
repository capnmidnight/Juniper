export declare class TimerTickEvent {
    t: number;
    dt: number;
    sdt: number;
    fps: number;
    frame: XRFrame;
    constructor();
    set(t: number, dt: number, frame: XRFrame): void;
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
