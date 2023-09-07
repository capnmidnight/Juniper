import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { JuniperAnalyserNode } from "./context/JuniperAnalyserNode";
import { SetIntervalTimer } from "@juniper-lib/timers/SetIntervalTimer";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
export declare class ActivityEvent extends TypedEvent<"activity"> {
    level: number;
    constructor();
}
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