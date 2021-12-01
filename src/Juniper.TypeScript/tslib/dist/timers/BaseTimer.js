import { arrayRemove } from "../collections/arrayRemove";
import { TimerTickEvent } from "./ITimer";
export class BaseTimer {
    _timer = null;
    _onTick;
    _targetFrameTime = Number.MAX_VALUE;
    _targetFPS = 0;
    lt = -1;
    tickHandlers = new Array();
    constructor(targetFrameRate) {
        this.targetFPS = targetFrameRate;
        const tickEvt = new TimerTickEvent();
        let dt = 0;
        this._onTick = (t, frame) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                tickEvt.set(t, dt, frame);
                this.tick(tickEvt);
            }
            this.lt = t;
        };
    }
    addTickHandler(onTick) {
        this.tickHandlers.push(onTick);
    }
    removeTickHandler(onTick) {
        arrayRemove(this.tickHandlers, onTick);
    }
    tick(evt) {
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
    stop() {
        this._timer = null;
        this.lt = -1;
    }
    get targetFPS() {
        return this._targetFPS;
    }
    set targetFPS(fps) {
        this._targetFPS = fps;
        this._targetFrameTime = 1000 / fps;
    }
    get targetFrameTime() {
        return this._targetFrameTime;
    }
}
