import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
export declare class PointerMouse extends BaseScreenPointerSinglePoint {
    allowPointerLock: boolean;
    private dz;
    private readonly keyMap;
    constructor(env: BaseEnvironment);
    protected updatePointerOrientation(): void;
    protected onUpdate(): void;
    get isPointerLocked(): boolean;
    get isPointerCaptured(): boolean;
    get canDragView(): boolean;
    get canTeleport(): boolean;
    lockPointer(): void;
    unlockPointer(): void;
    capturePointer(): void;
    releaseCapture(): void;
    vibrate(): void;
}
//# sourceMappingURL=PointerMouse.d.ts.map