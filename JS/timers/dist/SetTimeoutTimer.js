import { BaseTimer } from "./BaseTimer";
export class SetTimeoutTimer extends BaseTimer {
    constructor(targetFrameRate) {
        super(targetFrameRate);
    }
    start() {
        const updater = () => {
            this.timer = setTimeout(updater, this.targetFrameTime);
            this.onTick(performance.now());
        };
        this.timer = setTimeout(updater, this.targetFrameTime);
    }
    stop() {
        if (this.isRunning) {
            clearTimeout(this.timer);
            super.stop();
        }
    }
}
//# sourceMappingURL=SetTimeoutTimer.js.map