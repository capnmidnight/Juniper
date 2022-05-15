import { BaseTimer } from "./BaseTimer";

export class RequestAnimationFrameTimer extends BaseTimer<number> {
    constructor() {
        super();
    }

    start() {
        if (!this.isRunning) {
            const updater = (t: number) => {
                this.timer = requestAnimationFrame(updater);
                this.onTick(t);
            };
            this.timer = requestAnimationFrame(updater);
        }
    }

    override stop() {
        if (this.isRunning) {
            cancelAnimationFrame(this.timer);
            super.stop();
        }
    }
}
