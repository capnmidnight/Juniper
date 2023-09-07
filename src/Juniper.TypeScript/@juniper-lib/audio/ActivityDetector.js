import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { JuniperAnalyserNode } from "./context/JuniperAnalyserNode";
import { SetIntervalTimer } from "@juniper-lib/timers/SetIntervalTimer";
export class ActivityEvent extends TypedEvent {
    constructor() {
        super("activity");
        this.level = 0;
    }
}
export class ActivityDetector extends JuniperAnalyserNode {
    constructor(context) {
        super(context, {
            fftSize: 32,
            minDecibels: -70
        });
        this._level = 0;
        this.maxLevel = 0;
        this.activityEvt = new ActivityEvent();
        this.timer = new SetIntervalTimer(30);
        const buffer = new Uint8Array(this.frequencyBinCount);
        this.timer.addTickHandler(() => {
            this.getByteFrequencyData(buffer);
            this._level = Math.max(...buffer);
            if (isFinite(this._level)) {
                this.maxLevel = Math.max(this.maxLevel, this._level);
                if (this.maxLevel > 0) {
                    this._level /= this.maxLevel;
                }
            }
            this.activityEvt.level = this.level;
            this.dispatchEvent(this.activityEvt);
        });
    }
    get level() {
        return this._level;
    }
    start() {
        this.timer.start();
    }
    stop() {
        this.timer.stop();
    }
}
//# sourceMappingURL=ActivityDetector.js.map