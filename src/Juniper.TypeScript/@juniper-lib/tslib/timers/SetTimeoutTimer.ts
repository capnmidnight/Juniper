import { BaseTimer } from "./BaseTimer";

export class SetTimeoutTimer extends BaseTimer<number> {

    constructor(targetFrameRate: number) {
        super(targetFrameRate);
    }

    start() {
        const updater = () => {
            this.timer = setTimeout(updater, this.targetFrameTime) as unknown as number;
            this.onTick(performance.now());
        };
        this.timer = setTimeout(updater, this.targetFrameTime) as unknown as number;
    }

    override stop() {
        if (this.isRunning) {
            clearTimeout(this.timer);
            super.stop();
        }
    }
}
