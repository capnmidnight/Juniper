import { BaseTimer } from "./BaseTimer";
export class RequestAnimationFrameTimer extends BaseTimer {
    constructor() {
        super();
    }
    start() {
        if (!this.isRunning) {
            const updater = (t) => {
                this.timer = requestAnimationFrame(updater);
                this.onTick(t);
            };
            this.timer = requestAnimationFrame(updater);
        }
    }
    stop() {
        if (this.isRunning) {
            cancelAnimationFrame(this.timer);
            super.stop();
        }
    }
}
//# sourceMappingURL=RequestAnimationFrameTimer.js.map