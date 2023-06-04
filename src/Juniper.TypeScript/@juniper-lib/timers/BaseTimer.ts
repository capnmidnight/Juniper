import { arrayRemove } from "@juniper-lib/collections/arrays";
import { ITimer, TimerTickEvent } from "./ITimer";

export abstract class BaseTimer<TimerT> implements ITimer {
    protected timer: TimerT = null;
    protected onTick: (t: number) => void;
    private lt = -1;
    private tickHandlers = new Array<(evt: TimerTickEvent) => void>();

    constructor(targetFrameRate?: number) {
        this.targetFPS = targetFrameRate;
        const tickEvt = new TimerTickEvent();
        let dt = 0;
        this.onTick = (t: number) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                tickEvt.set(t, dt);
                this.tick(tickEvt);
            }
            this.lt = t;
        };
    }


    private _targetFPS: number = null;
    get targetFPS() {
        return this._targetFPS;
    }

    set targetFPS(v: number) {
        this._targetFPS = v;
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
        return this.timer != null;
    }

    abstract start(): void;

    stop() {
        this.timer = null;
        this.lt = -1;
    }

    protected get targetFrameTime() {
        return 1000 / this.targetFPS;
    }
}