import { BaseVariableTimer } from "./BaseVariableTimer";
export class SetIntervalTimer extends BaseVariableTimer {
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