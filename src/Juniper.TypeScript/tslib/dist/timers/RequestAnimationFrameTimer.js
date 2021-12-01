import { BaseTimer } from "./BaseTimer";
export class RequestAnimationFrameTimer extends BaseTimer {
    _timerSource = globalThis;
    constructor() {
        super(120);
    }
    get timerSource() {
        return this._timerSource;
    }
    set timerSource(v) {
        if (v !== this.timerSource) {
            this.stop();
            this._timerSource = v;
            this.start();
        }
    }
    start() {
        const updater = (t, frame) => {
            this._timer = this.timerSource.requestAnimationFrame(updater);
            this._onTick(t, frame);
        };
        this._timer = this.timerSource.requestAnimationFrame(updater);
    }
    stop() {
        if (this._timer) {
            this.timerSource.cancelAnimationFrame(this._timer);
            super.stop();
        }
    }
}
