import { BaseTimer } from "./BaseTimer";
export class BaseManagedTimer extends BaseTimer {
    get isRunning() { return this.timer != null; }
    constructor() {
        super();
        this.timer = null;
        let dt = 0;
        this.onTick = (t) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                this.tickEvt.set(t, dt);
                for (const handler of this.tickHandlers) {
                    handler(this.tickEvt);
                }
            }
            this.lt = t;
        };
    }
    stop() {
        this.timer = null;
        super.stop();
    }
}
//# sourceMappingURL=BaseManagedTimer.js.map