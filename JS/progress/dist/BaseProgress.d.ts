import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
import type { IProgress } from "./IProgress";
export declare class BaseProgress<T extends TypedEventMap<string> = TypedEventMap<string>> extends TypedEventTarget<T> implements IProgress {
    private readonly attached;
    private soFar;
    private total;
    private msg;
    private est;
    protected get p(): number;
    report(soFar: number, total: number, msg?: string, est?: number): void;
    attach(prog: IProgress): void;
    clear(): void;
    start(msg?: string): void;
    end(msg?: string): void;
    private _clear;
}
//# sourceMappingURL=BaseProgress.d.ts.map