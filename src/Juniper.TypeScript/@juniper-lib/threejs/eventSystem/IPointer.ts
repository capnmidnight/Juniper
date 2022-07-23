import type { VirtualButton } from "@juniper-lib/threejs/eventSystem/VirtualButton";
import type { PointerID, PointerType, TypedEventBase } from "@juniper-lib/tslib";
import type { BaseCursor } from "./BaseCursor";
import type { Pointer3DEvents } from "./Pointer3DEvent";
import type { RayTarget } from "./RayTarget";

export interface IPointer extends TypedEventBase<Pointer3DEvents> {
    id: PointerID;
    type: PointerType;
    cursor: BaseCursor;
    direction: THREE.Vector3;
    origin: THREE.Vector3;
    up: THREE.Vector3;
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
