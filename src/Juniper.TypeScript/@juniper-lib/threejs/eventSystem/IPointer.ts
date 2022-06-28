import type { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import type { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseCursor } from "./BaseCursor";
import type { MouseButtons } from "./MouseButton";

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

    buttons: MouseButtons;
    dragging: boolean;

    needsUpdate: boolean;
    update(): void;
    isPressed(buttons: VirtualButtons): boolean;

    vibrate(): void;
    updateCursor(avatarHeadPos: THREE.Vector3, hit: THREE.Intersection, dist: number): void;
}
