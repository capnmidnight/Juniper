import { TypedEvent } from "@juniper-lib/events";
import { lerp } from "@juniper-lib/util";
export class BaseTimerTickEvent extends TypedEvent {
    constructor() {
        super("update");
        this.t = 0;
        this.dt = 0;
        this.sdt = 0;
        this.fps = 0;
    }
    set(t, dt) {
        this.t = t;
        this.dt = dt;
        this.sdt = lerp(this.sdt, dt, 0.01);
        if (dt > 0) {
            this.fps = 1000 / dt;
        }
    }
}
export class TimerTickEvent extends BaseTimerTickEvent {
    constructor() {
        super();
        Object.seal(this);
    }
}
//# sourceMappingURL=ITimer.js.map