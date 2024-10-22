import { ElementChild } from "@juniper-lib/dom";
import { IProgress } from "@juniper-lib/progress";
export declare class TypedProgressBar extends HTMLElement implements IProgress {
    #private;
    constructor();
    connectedCallback(): void;
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    clear(): void;
    start(msg?: string): void;
    end(msg?: string): void;
    static install(): import("@juniper-lib/dom").ElementFactory<TypedProgressBar>;
}
export declare function ProgressBar(...rest: ElementChild<TypedProgressBar>[]): TypedProgressBar;
//# sourceMappingURL=ProgressBar.d.ts.map