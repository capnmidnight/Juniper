import type { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import type { Vector3 } from "three";
import type { BufferReaderWriter } from "../../BufferReaderWriter";
import type { BaseCursor3D } from "../cursors/BaseCursor3D";
import type { PointerID, PointerType } from "../Pointers";
import type { RayTarget } from "../RayTarget";
import type { Pointer3DEvents } from "./Pointer3DEvent";
import type { VirtualButton } from "./VirtualButton";
export interface IPointer extends TypedEventTarget<Pointer3DEvents> {
    id: PointerID;
    type: PointerType;
    cursor: BaseCursor3D;
    direction: Vector3;
    origin: Vector3;
    up: Vector3;
    canMoveView: boolean;
    canDragView: boolean;
    canTeleport: boolean;
    enabled: boolean;
    isActive: boolean;
    canSend: boolean;
    rayTarget: RayTarget;
    isPressed(button: VirtualButton): boolean;
    needsUpdate: boolean;
    update(): void;
    vibrate(): void;
    bufferSize: number;
    writeState(buffer: BufferReaderWriter): void;
}
//# sourceMappingURL=IPointer.d.ts.map