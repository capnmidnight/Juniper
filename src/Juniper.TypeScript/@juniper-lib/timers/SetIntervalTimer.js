import { BaseTimer } from "./BaseTimer";
export class SetIntervalTimer extends BaseTimer {
    constructor(targetFrameRate) {
        super(targetFrameRate);
    }
    start() {
        this.timer = setInterval(() => this.onTick(performance.now()), this.targetFrameTime);
    }
    stop() {
        if (this.timer) {
            clearInterval(this.timer);
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
//# sourceMappingURL=SetIntervalTimer.js.map