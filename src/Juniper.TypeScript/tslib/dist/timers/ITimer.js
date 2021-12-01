import { lerp } from "../math/lerp";
export class TimerTickEvent {
    t = 0;
    dt = 0;
    sdt = 0;
    fps = 0;
    frame = null;
    constructor() {
        Object.seal(this);
    }
    set(t, dt, frame) {
        this.t = t;
        this.dt = dt;
        this.sdt = lerp(this.sdt, dt, 0.01);
        this.frame = frame;
        if (dt > 0) {
            this.fps = 1000 / dt;
        }
    }
}
