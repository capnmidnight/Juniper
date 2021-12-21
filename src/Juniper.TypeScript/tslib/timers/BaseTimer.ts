import { arrayRemove } from "../collections/arrayRemove";
import { ITimer, TimerTickEvent } from "./ITimer";

export abstract class BaseTimer<TimerT>
    implements ITimer {
    protected _timer: TimerT = null;
    protected _onTick: (t: number, frame?: XRFrame) => void;
    protected _targetFrameTime = Number.MAX_VALUE;
    private _targetFPS = 0;
    private lt: number = -1;
    private tickHandlers = new Array<(evt: TimerTickEvent) => void>();

    constructor(targetFrameRate: number) {

        this.targetFPS = targetFrameRate;
        const tickEvt = new TimerTickEvent();
        let dt = 0;
        this._onTick = (t: number, frame?: XRFrame) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                tickEvt.set(t, dt, frame);
                this.tick(tickEvt);
            }
            this.lt = t;
        };
    }

    addTickHandler(onTick: (evt: TimerTickEvent) => void): void {
        this.tickHandlers.push(onTick);
    }

    removeTickHandler(onTick: (evt: TimerTickEvent) => void): void {
        arrayRemove(this.tickHandlers, onTick);
    }

    private tick(evt: TimerTickEvent): void {
        for (const handler of this.tickHandlers) {
            handler(evt);
        }
    }

    restart() {
        this.stop();
        this.start();
    }

    get isRunning() {
        return this._timer != null;
    }

    abstract start(): void;

    stop() {
        this._timer = null;
        this.lt = -1;
    }

    get targetFPS() {
        return this._targetFPS;
    }

    set targetFPS(fps: number) {
        this._targetFPS = fps;
        this._targetFrameTime = 1000 / fps;
    }

    get targetFrameTime() {
        return this._targetFrameTime;
    }
}