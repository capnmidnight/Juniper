import { BaseTimer } from "./BaseTimer";


export abstract class BaseManagedTimer<TimerT> extends BaseTimer<(t: number) => void> {

    protected timer: TimerT = null;
    get isRunning() { return this.timer != null; }

    constructor() {
        super();

        let dt = 0;
        this.onTick = (t: number) => {
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

    override stop() {
        this.timer = null;
        super.stop();
    }
}
