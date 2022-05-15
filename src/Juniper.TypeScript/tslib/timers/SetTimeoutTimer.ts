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
        this._timer= setTimeout(updater, this.targetFrameTime);
    }

    override stop() {
        if (this._timer {
            clearTimeout(this._timer;
            super.stop();
        }
    }
}
