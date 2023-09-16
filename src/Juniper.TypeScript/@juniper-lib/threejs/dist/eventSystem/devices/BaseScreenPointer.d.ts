import { Vector2 } from "three";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import type { BaseCursor3D } from "../cursors/BaseCursor3D";
import { PointerID, PointerType } from "../Pointers";
import { BasePointer } from "./BasePointer";
export declare abstract class BaseScreenPointer extends BasePointer {
    element: HTMLCanvasElement;
    protected readonly position: Vector2;
    protected readonly motion: Vector2;
    private readonly uv;
    private readonly duv;
    private readonly uvComp;
    private readonly uvOff;
    constructor(type: PointerType, id: PointerID, env: BaseEnvironment, cursor: BaseCursor3D);
    protected onCheckEvent(evt: PointerEvent): boolean;
    protected onReadEvent(_evt: PointerEvent): void;
    protected updatePointerOrientation(): void;
    protected onUpdate(): void;
}
//# sourceMappingURL=BaseScreenPointer.d.ts.map