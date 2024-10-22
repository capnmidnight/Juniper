import { BaseTimerTickEvent, ITimer } from "@juniper-lib/timers";
import { WebGLRenderer } from "three";
export declare class XRTimerTickEvent extends BaseTimerTickEvent {
    frame?: XRFrame;
    constructor();
    set(t: number, dt: number, frame?: XRFrame): void;
}
export declare class XRTimer implements ITimer {
    private readonly renderer;
    private tickHandlers;
    private _onTick;
    private lt;
    constructor(renderer: WebGLRenderer);
    private _isRunning;
    get isRunning(): boolean;
    restart(): void;
    addTickHandler(onTick: (evt: XRTimerTickEvent) => void): void;
    removeTickHandler(onTick: (evt: XRTimerTickEvent) => void): void;
    private setAnimationLoop;
    start(): void;
    stop(): void;
    private tick;
}
//# sourceMappingURL=XRTimer.d.ts.map