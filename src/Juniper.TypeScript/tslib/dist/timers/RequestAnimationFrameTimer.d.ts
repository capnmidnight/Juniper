import { BaseTimer } from "./BaseTimer";
declare type TimerSource = typeof globalThis | XRSession;
export declare class RequestAnimationFrameTimer extends BaseTimer<number> {
    private _timerSource;
    constructor();
    get timerSource(): TimerSource;
    set timerSource(v: TimerSource);
    start(): void;
    stop(): void;
}
export {};
