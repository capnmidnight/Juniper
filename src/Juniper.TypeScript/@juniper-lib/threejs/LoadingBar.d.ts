import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { ErsatzObject } from "./objects";
export declare class LoadingBar extends BaseProgress implements IProgress, ErsatzObject {
    private readonly valueBar;
    private value;
    private targetValue;
    readonly object: import("three").Object3D<import("three").Event>;
    constructor();
    private _enabled;
    get enabled(): boolean;
    set enabled(v: boolean);
    report(soFar: number, total: number, msg?: string): void;
    update(dt: number): void;
}
//# sourceMappingURL=LoadingBar.d.ts.map