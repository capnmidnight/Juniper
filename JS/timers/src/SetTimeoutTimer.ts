import { BaseVariableTimer } from "./BaseVariableTimer";

export class SetTimeoutTimer extends BaseVariableTimer<number> {
    
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
