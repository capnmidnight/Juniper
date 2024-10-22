import { SetIntervalTimer } from "@juniper-lib/timers";
import { ActivityEvent } from "./ActivityEvent";
import { JuniperAnalyserNode } from "./context/JuniperAnalyserNode";
import { JuniperAudioContext } from "./context/JuniperAudioContext";

export class ActivityDetector extends JuniperAnalyserNode<{
    activity: ActivityEvent;
}> {

    private _level = 0;
    private maxLevel = 0;
    private readonly activityEvt = new ActivityEvent();
    protected readonly timer = new SetIntervalTimer(30);

    constructor(context: JuniperAudioContext) {
        super(context, {
            fftSize: 32,
            minDecibels: -70
        });

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
