import { arrayRemove } from "@juniper-lib/util";
import { BaseTimerTickEvent } from "@juniper-lib/timers";
import { isDefined } from "@juniper-lib/util";
export class XRTimerTickEvent extends BaseTimerTickEvent {
    constructor() {
        super();
        this.frame = null;
        Object.seal(this);
    }
    set(t, dt, frame) {
        super.set(t, dt);
        this.frame = frame;
    }
}
export class XRTimer {
    constructor(renderer) {
        this.renderer = renderer;
        this.tickHandlers = new Array();
        this.lt = -1;
        this._isRunning = false;
        const tickEvt = new XRTimerTickEvent();
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
    get isRunning() {
        return this._isRunning;
    }
    restart() {
        this.stop();
        this.start();
    }
    addTickHandler(onTick) {
        this.tickHandlers.push(onTick);
    }
    removeTickHandler(onTick) {
        arrayRemove(this.tickHandlers, onTick);
    }
    setAnimationLoop(loop) {
        this.renderer.setAnimationLoop(loop);
        this._isRunning = isDefined(loop);
    }
    start() {
        if (!this.isRunning) {
            this.setAnimationLoop(this._onTick);
        }
    }
    stop() {
        if (this.isRunning) {
            this.setAnimationLoop(null);
            this.lt = -1;
        }
    }
    tick(evt) {
        for (const handler of this.tickHandlers) {
            handler(evt);
        }
    }
}
//# sourceMappingURL=XRTimer.js.map