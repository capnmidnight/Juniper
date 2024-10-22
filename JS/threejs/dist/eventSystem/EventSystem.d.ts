import { TypedEventTarget } from "@juniper-lib/events";
import { Intersection, Vector3 } from "three";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import type { IPointer } from "./devices/IPointer";
import { Pointer3DEvents } from "./devices/Pointer3DEvent";
import { PointerHand } from "./devices/PointerHand";
import { PointerMouse } from "./devices/PointerMouse";
import { PointerNose } from "./devices/PointerNose";
import { PointerPen } from "./devices/PointerPen";
import { PointerTouch } from "./devices/PointerTouch";
export type IntersectionSortFunction = (a: Intersection, b: Intersection) => number;
export declare class EventSystem extends TypedEventTarget<Pointer3DEvents> {
    private readonly env;
    private readonly raycaster;
    readonly mouse: PointerMouse;
    readonly pen: PointerPen;
    readonly touches: PointerTouch;
    readonly nose: PointerNose;
    readonly hands: PointerHand[];
    readonly pointers: IPointer[];
    private readonly hits;
    private readonly queue;
    private readonly targetsFound;
    private readonly targets;
    private customSortFunction;
    set sortFunction(func: IntersectionSortFunction);
    get sortFunction(): IntersectionSortFunction;
    constructor(env: BaseEnvironment<unknown>);
    checkXRMouse(): void;
    refreshCursors(): void;
    fireRay(origin: Vector3, direction: Vector3): Intersection;
    update(): void;
}
//# sourceMappingURL=EventSystem.d.ts.map