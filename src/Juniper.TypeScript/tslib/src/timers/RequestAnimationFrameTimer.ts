import { BaseTimer } from "./BaseTimer";

type TimerSource = typeof globalThis | XRSession;

export class RequestAnimationFrameTimer extends BaseTimer<number> {

    private _timerSource: TimerSource = globalThis;

    constructor() {
        super(120);
    }

    get timerSource() {
        return this._timerSource;
    }

    set timerSource(v: TimerSource) {
        if (v !== this.timerSource) {
            this.stop();
            this._timerSource = v;
            this.start();
        }
    }

    start() {
        const updater = (t: number, frame?: XRFrame) => {
            this._timer = this.timerSource.requestAnimationFrame(updater);
            this._onTick(t, frame);
        };
        this._timer = this.timerSource.requestAnimationFrame(updater);
    }

    override stop() {
        if (this._timer) {
            this.timerSource.cancelAnimationFrame(this._timer);
            super.stop();
        }
    }
}
