import { BaseTimer } from "./BaseTimer";

export class SetIntervalTimer extends BaseTimer<number> {

    constructor(targetFrameRate: number) {
        super(targetFrameRate);
    }

    start() {
        this._timer = setInterval(
            () => this._onTick(performance.now()),
            this._targetFrameTime);
    }

    override stop() {
        if (this._timer) {
            clearInterval(this._timer);
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

