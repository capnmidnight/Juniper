import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { PointerID, PointerType } from "../Pointers";
import { BaseScreenPointer } from "./BaseScreenPointer";
export declare abstract class BaseScreenPointerSinglePoint extends BaseScreenPointer {
    protected pointerID: number;
    constructor(type: PointerType, id: PointerID, env: BaseEnvironment);
    protected onCheckEvent(evt: PointerEvent): boolean;
    private lastX;
    private lastY;
    protected onReadEvent(evt: PointerEvent): void;
}
//# sourceMappingURL=BaseScreenPointerSinglePoint.d.ts.map