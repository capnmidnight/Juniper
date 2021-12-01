import { BaseTimer } from "./BaseTimer";
export class SetIntervalTimer extends BaseTimer {
    constructor(targetFrameRate) {
        super(targetFrameRate);
    }
    start() {
        this._timer = setInterval(() => this._onTick(performance.now()), this._targetFrameTime);
    }
    stop() {
        if (this._timer) {
            clearInterval(this._timer);
            super.stop();
        }
    }
    get targetFPS() {
        return super.targetFPS;
    }
    set targetFPS(fps) {
        super.targetFPS = fps;
        if (this.isRunning) {
            this.restart();
        }
    }
}
