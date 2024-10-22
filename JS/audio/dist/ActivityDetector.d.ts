import { SetIntervalTimer } from "@juniper-lib/timers";
import { ActivityEvent } from "./ActivityEvent";
import { JuniperAnalyserNode } from "./context/JuniperAnalyserNode";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
export declare class ActivityDetector extends JuniperAnalyserNode<{
    activity: ActivityEvent;
}> {
    private _level;
    private maxLevel;
    private readonly activityEvt;
    protected readonly timer: SetIntervalTimer;
    constructor(context: JuniperAudioContext);
    get level(): number;
    start(): void;
    stop(): void;
}
//# sourceMappingURL=ActivityDetector.d.ts.map