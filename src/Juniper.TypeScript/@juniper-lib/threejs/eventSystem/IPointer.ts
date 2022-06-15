import { PointerState } from "@juniper-lib/threejs/eventSystem/PointerState";
import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseCursor } from "./BaseCursor";

export type PointerType = "mouse"
    | "touch"
    | "gamepad"
    | "pen"
    | "hand"
    | "remote";

export interface IPointer {
    name: PointerName;
    type: PointerType;
    cursor: BaseCursor;
    direction: THREE.Vector3;
    origin: THREE.Vector3;
    canMoveView: boolean;
    enabled: boolean;

    curHit: THREE.Intersection;
    hoveredHit: THREE.Intersection;
    pressedHit: THREE.Intersection;
    draggedHit: THREE.Intersection;

    state: PointerState;

    needsUpdate: boolean;
    update(): void;
    recheck(): void;
    isPressed(buttons: VirtualButtons): boolean;
    isActive: boolean;

    vibrate(): void;
    updateCursor(avatarHeadPos: THREE.Vector3, hit: THREE.Intersection, dist: number): void;
}
