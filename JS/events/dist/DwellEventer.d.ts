import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";
export declare class DwellEvent extends TypedEvent<"dwell"> {
    readonly dwellTimeSeconds: number;
    constructor(dwellTimeSeconds: number);
}
export declare class DwellEventer extends TypedEventTarget<{
    dwell: DwellEvent;
}> {
    #private;
    constructor(minTimeSeconds?: number, graceTimeSeconds?: number);
    start(): void;
    stop(): void;
}
//# sourceMappingURL=DwellEventer.d.ts.map