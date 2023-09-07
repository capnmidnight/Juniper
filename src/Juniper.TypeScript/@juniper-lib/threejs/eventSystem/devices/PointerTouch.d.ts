import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseScreenPointer } from "./BaseScreenPointer";
export declare class PointerTouch extends BaseScreenPointer {
    private dz;
    private lastZ;
    private readonly points;
    constructor(env: BaseEnvironment);
    get enabled(): boolean;
    set enabled(v: boolean);
    private readonly lastXs;
    private readonly lastYs;
    protected onReadEvent(evt: PointerEvent): void;
    protected onUpdate(): void;
    private _canVibrate;
    private get canVibrate();
    vibrate(): void;
}
//# sourceMappingURL=PointerTouch.d.ts.map