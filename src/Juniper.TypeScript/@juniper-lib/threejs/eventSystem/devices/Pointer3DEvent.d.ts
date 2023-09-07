import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { Intersection, Vector3 } from "three";
import type { RayTarget } from "../RayTarget";
import type { IPointer } from "./IPointer";
import type { PointerEventTypes } from "./PointerEventTypes";
export declare class Pointer3DEvent<T extends PointerEventTypes = PointerEventTypes> extends TypedEvent<T> {
    readonly pointer: IPointer;
    private _hit;
    private _point;
    private _distance;
    private _rayTarget;
    constructor(type: T, pointer: IPointer);
    set(v: Intersection, t: RayTarget): void;
    get hit(): Intersection;
    get rayTarget(): RayTarget;
    get point(): Vector3;
    get distance(): number;
}
export type Pointer3DEvents = {
    move: Pointer3DEvent<"move">;
    enter: Pointer3DEvent<"enter">;
    exit: Pointer3DEvent<"exit">;
    up: Pointer3DEvent<"up">;
    down: Pointer3DEvent<"down">;
    click: Pointer3DEvent<"click">;
};
//# sourceMappingURL=Pointer3DEvent.d.ts.map