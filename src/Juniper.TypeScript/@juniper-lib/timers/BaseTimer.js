import { arrayRemove } from "@juniper-lib/collections/arrays";
import { TimerTickEvent } from "./ITimer";
export class BaseTimer {
    constructor(targetFrameRate) {
        this.timer = null;
        this.lt = -1;
        this.tickHandlers = new Array();
        this._targetFPS = null;
        this.targetFPS = targetFrameRate;
        const tickEvt = new TimerTickEvent();
        let dt = 0;
        this.onTick = (t) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                tickEvt.set(t, dt);
                this.tick(tickEvt);
            }
            this.lt = t;
        };
    }
    get targetFPS() {
        return this._targetFPS;
    }
    set targetFPS(v) {
        this._targetFPS = v;
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
        return this.timer != null;
    }
    stop() {
        this.timer = null;
        this.lt = -1;
    }
    get targetFrameTime() {
        return 1000 / this.targetFPS;
    }
}
//# sourceMappingURL=BaseTimer.js.map