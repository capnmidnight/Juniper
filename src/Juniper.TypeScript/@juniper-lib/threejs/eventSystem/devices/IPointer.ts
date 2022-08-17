import type { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import type { PointerID, PointerType } from "@juniper-lib/tslib/events/Pointers";
import { Vector3 } from "three";
import type { BaseCursor } from "../cursors/BaseCursor";
import type { RayTarget } from "../RayTarget";
import type { Pointer3DEvents } from "./Pointer3DEvent";
import type { VirtualButton } from "./VirtualButton";

export interface IPointer extends TypedEventBase<Pointer3DEvents> {
    id: PointerID;
    type: PointerType;
    cursor: BaseCursor;
    direction: Vector3;
    origin: Vector3;
    up: Vector3;
    canMoveView: boolean;
    canDragView: boolean;
    canTeleport: boolean;
    enabled: boolean;

    rayTarget: RayTarget;

    isPressed(button: VirtualButton): boolean;

    needsUpdate: boolean;
    update(): void;

    vibrate(): void;
}
