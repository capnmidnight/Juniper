import { BaseTimer } from "./BaseTimer";

export class SetTimeoutTimer extends BaseTimer<number> {

    constructor(targetFrameRate: number) {
        super(targetFrameRate);
    }

    start() {
        const updater = () => {
            this.timer = setTimeout(updater, this.targetFrameTime);
            this.onTick(performance.now());
        };
        this.timer = setTimeout(updater, this.targetFrameTime);
    }

    override stop() {
        if (this.isRunning) {
            clearTimeout(this.timer);
            super.stop();
        }
    }
}
