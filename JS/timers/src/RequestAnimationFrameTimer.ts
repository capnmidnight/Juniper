import { BaseManagedTimer } from "./BaseManagedTimer";

export class RequestAnimationFrameTimer extends BaseManagedTimer<number> {

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