import { BaseTimer } from "./BaseTimer";
export class SetTimeoutTimer extends BaseTimer {
    constructor(targetFrameRate) {
        super(targetFrameRate);
    }
    start() {
        const updater = () => {
            this._timer = setTimeout(updater, this._targetFrameTime);
            this._onTick(performance.now());
        };
        this._timer = setTimeout(updater, this._targetFrameTime);
    }
    stop() {
        if (this._timer) {
            clearTimeout(this._timer);
            super.stop();
        }
    }
}
