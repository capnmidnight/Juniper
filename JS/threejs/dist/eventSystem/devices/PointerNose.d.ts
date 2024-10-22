import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BasePointer } from "./BasePointer";
export declare class PointerNose extends BasePointer {
    private readonly point;
    private readonly lastPoint;
    constructor(env: BaseEnvironment);
    vibrate(): void;
    get canSend(): boolean;
    protected updatePointerOrientation(): void;
}
//# sourceMappingURL=PointerNose.d.ts.map