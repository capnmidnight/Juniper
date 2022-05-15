import { BaseTimer } from "./BaseTimer";

export class SetIntervalTimer extends BaseTimer<number> {

    constructor(targetFrameRate: number) {
        super(targetFrameRate);
    }

    start() {
        this.timer = setInterval(
            () => this.onTick(performance.now()),
            this.targetFrameTime);
    }

    override stop() {
        if (this.timer) {
            clearInterval(this.timer);
            super.stop();
        }
    }

    override get targetFPS() {
        return super.targetFPS;
    }

    override set targetFPS(fps) {
        super.targetFPS = fps;
        if (this.isRunning) {
            this.restart();
        }
    }
}

