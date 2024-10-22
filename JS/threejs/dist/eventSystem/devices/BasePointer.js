import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { Vector3 } from "three";
import { objGraph } from "../../objects";
import { CursorXRMouse } from "../cursors/CursorXRMouse";
import { PointerID } from "../Pointers";
import { getRayTarget } from "../RayTarget";
import { Pointer3DEvent } from "./Pointer3DEvent";
import { VirtualButton } from "./VirtualButton";
const MAX_DRAG_DISTANCE = 5;
const ZERO = new Vector3();
export class BasePointer extends TypedEventTarget {
    constructor(type, id, env, cursor) {
        super();
        this.type = type;
        this.id = id;
        this.env = env;
        this.origin = new Vector3();
        this.direction = new Vector3();
        this.up = new Vector3(0, 1, 0);
        this.canMoveView = false;
        this.mayTeleport = false;
        this.buttons = 0;
        this._isActive = false;
        this.moveDistance = 0;
        this.pointerEvents = new Map();
        this.lastButtons = 0;
        this.canClick = false;
        this.dragDistance = 0;
        this._enabled = false;
        this._cursor = null;
        this._curHit = null;
        this._curTarget = null;
        this._hoveredHit = null;
        this._hoveredTarget = null;
        this._cursor = cursor;
        if (this.cursor) {
            this.cursor.visible = false;
        }
    }
    get isActive() {
        return this._isActive;
    }
    get canSend() {
        return this.enabled
            && this.isActive;
    }
    get curHit() {
        return this._curHit;
    }
    get curTarget() {
        return this._curTarget;
    }
    get hoveredHit() {
        return this._hoveredHit;
    }
    set hoveredHit(v) {
        if (v !== this.hoveredHit) {
            const t = getRayTarget(v);
            this._hoveredHit = v;
            this._hoveredTarget = t;
        }
    }
    get name() {
        return PointerID[this.id];
    }
    get rayTarget() {
        return this._hoveredTarget;
    }
    get cursor() {
        return this._cursor;
    }
    set cursor(newCursor) {
        if (newCursor !== this.cursor) {
            const oldCursor = this.cursor;
            const oldName = this.cursor && this.cursor.object && this.cursor.object.name || "cursor";
            const oldParent = oldCursor && oldCursor.object && oldCursor.object.parent;
            if (oldParent) {
                oldCursor.object.removeFromParent();
            }
            if (newCursor) {
                newCursor.object.name = oldName;
                if (oldCursor instanceof CursorXRMouse) {
                    oldCursor.cursor = newCursor;
                    if (oldParent) {
                        objGraph(oldParent, oldCursor);
                    }
                }
                else {
                    this._cursor = newCursor;
                    if (oldCursor) {
                        if (oldParent) {
                            objGraph(oldParent, newCursor);
                        }
                        newCursor.style = oldCursor.style;
                        newCursor.visible = oldCursor.visible;
                    }
                }
            }
        }
    }
    get needsUpdate() {
        return this.enabled
            && this._isActive;
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        this._enabled = v;
        if (this.cursor) {
            this.cursor.visible = v;
        }
    }
    setButton(button, pressed) {
        this.lastButtons = this.buttons;
        const mask = 1 << button;
        if (pressed) {
            this.buttons |= mask;
        }
        else {
            this.buttons &= ~mask;
        }
        if (pressed) {
            this.canClick = true;
            this.dragDistance = 0;
            this.env.avatar.setMode(this);
            this.setEventState("down");
        }
        else {
            if (this.canClick) {
                const curButtons = this.buttons;
                this.buttons = this.lastButtons;
                this.setEventState("click");
                this.buttons = curButtons;
            }
            this.setEventState("up");
        }
    }
    isPressed(button) {
        const mask = 1 << button;
        return (this.buttons & mask) !== 0;
    }
    wasPressed(button) {
        const mask = 1 << button;
        return (this.lastButtons & mask) !== 0;
    }
    fireRay(origin, direction) {
        const minHit = this.env.eventSys.fireRay(origin, direction);
        if (minHit !== this.curHit) {
            const t = getRayTarget(minHit);
            this._curHit = minHit;
            this._curTarget = t;
        }
    }
    getEvent(type) {
        if (!this.pointerEvents.has(type)) {
            this.pointerEvents.set(type, new Pointer3DEvent(type, this));
        }
        const evt = this.pointerEvents.get(type);
        if (this.hoveredHit) {
            evt.set(this.hoveredHit, this.rayTarget);
        }
        else if (this.curHit) {
            evt.set(this.curHit, this.curTarget);
        }
        else {
            evt.set(null, null);
        }
        if (evt.hit) {
            const lastHit = this.curHit || this.hoveredHit;
            if (lastHit && evt.hit !== lastHit) {
                evt.hit.uv = lastHit.uv;
            }
        }
        return evt;
    }
    update() {
        if (this.needsUpdate) {
            this.onUpdate();
        }
    }
    onUpdate() {
        this.updatePointerOrientation();
        const primaryPressed = this.isPressed(VirtualButton.Primary);
        if (this.moveDistance > 0 || primaryPressed) {
            if (primaryPressed) {
                this.dragDistance += this.moveDistance;
                if (this.dragDistance > MAX_DRAG_DISTANCE) {
                    this.canClick = false;
                }
            }
            this.setEventState("move");
        }
        this.moveDistance = 0;
    }
    setEventState(eventType) {
        this.fireRay(this.origin, this.direction);
        if (this.curTarget === this.rayTarget) {
            this.hoveredHit = this.curHit;
        }
        else {
            const isPressed = this.isPressed(VirtualButton.Primary);
            const wasPressed = this.wasPressed(VirtualButton.Primary);
            const openMove = eventType === "move" && !isPressed;
            const primaryDown = eventType === "down" && isPressed && !wasPressed;
            const primaryUp = eventType === "up" && !isPressed && wasPressed;
            if (openMove
                || primaryDown
                || primaryUp) {
                if (this.rayTarget) {
                    const upEvt = this.getEvent("up");
                    this.rayTarget.dispatchEvent(upEvt);
                    const exitEvt = this.getEvent("exit");
                    this.dispatchEvent(exitEvt);
                    this.rayTarget.dispatchEvent(exitEvt);
                }
                this.hoveredHit = this.curHit;
                if (this.rayTarget) {
                    const enterEvt = this.getEvent("enter");
                    this.dispatchEvent(enterEvt);
                    this.rayTarget.dispatchEvent(enterEvt);
                }
            }
            if (this.hoveredHit) {
                this.hoveredHit.point
                    .copy(this.direction)
                    .multiplyScalar(this.hoveredHit.distance)
                    .add(this.origin);
            }
        }
        const evt = this.getEvent(eventType);
        this.dispatchEvent(evt);
        if (evt.rayTarget
            && (eventType !== "click"
                || evt.rayTarget.clickable
                || evt.rayTarget.navigable)) {
            if (eventType === "click"
                && evt.rayTarget.clickable) {
                this.vibrate();
            }
            if (evt.rayTarget.enabled) {
                evt.rayTarget.dispatchEvent(evt);
            }
        }
        this.updateCursor(this.env.avatar.worldPos, ZERO, true, 2);
    }
    get canDragView() {
        return this.canMoveView;
    }
    get canTeleport() {
        return this.mayTeleport;
    }
    updateCursor(avatarHeadPos, comfortOffset, isLocal, defaultDistance) {
        if (this.cursor) {
            this.cursor.update(avatarHeadPos, comfortOffset, this.hoveredHit || this.curHit, this.rayTarget || this.curTarget, defaultDistance, isLocal, this.canDragView, this.canTeleport, this.origin, this.direction, this.isPressed(VirtualButton.Primary));
        }
    }
    get bufferSize() {
        //   pointerID = 1 byte
        // + pointer pose =
        //   origin, direction, up = 3 vectors
        // * x, y, z = 3 components per vector
        // * float32 = 4 bytes per component
        //         = 36 bytes
        return 37;
    }
    writeState(buffer) {
        buffer.writeUint8(this.id);
        buffer.writeVector48(this.origin);
        buffer.writeVector48(this.direction);
        buffer.writeVector48(this.up);
    }
}
//# sourceMappingURL=BasePointer.js.map