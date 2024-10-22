import { arrayRemove } from "@juniper-lib/util";
import { TimerTickEvent } from "./ITimer";
export class BaseTimer {
    constructor() {
        this.lt = -1;
        this.tickEvt = new TimerTickEvent();
        this.tickHandlers = new Array();
    }
    ;
    addTickHandler(onTick) {
        this.tickHandlers.push(onTick);
    }
    removeTickHandler(onTick) {
        arrayRemove(this.tickHandlers, onTick);
    }
    restart() {
        this.stop();
        this.start();
    }
    stop() {
        this.lt = -1;
    }
}
//# sourceMappingURL=BaseTimer.js.map