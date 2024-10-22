import { TypedEvent, TypedEventMap, TypedEventTarget } from "@juniper-lib/events";
import type { IProgress } from "./IProgress";
export declare class ProgressEvent extends TypedEvent<"progress"> {
    readonly progress: BaseProgress;
    constructor(progress: BaseProgress);
}
export declare class BaseProgress<T extends TypedEventMap<string> = TypedEventMap<string>> extends TypedEventTarget<T & {
    "progress": ProgressEvent;
}> implements IProgress {
    #private;
    get message(): string;
    get estimate(): number;
    get basicMessage(): string;
    constructor();
    get p(): number;
    report(soFar: number, total: number, msg?: string, est?: number): void;
    attach(prog: IProgress): void;
    clear(): void;
    start(msg?: string): void;
    end(msg?: string): void;
}
//# sourceMappingURL=BaseProgress.d.ts.map