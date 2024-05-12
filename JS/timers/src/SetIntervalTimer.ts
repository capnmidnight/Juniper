import { BaseVariableTimer } from "./BaseVariableTimer";

export class SetIntervalTimer extends BaseVariableTimer<number> {
    
    start() {
        this.timer = setInterval(
            () => this.onTick(performance.now()),
            this.targetFrameTime) as any as number;
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

