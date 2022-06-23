import { MouseButtons } from "@juniper-lib/threejs/eventSystem/MouseButton";
import { SourcePointerEventTypes } from "@juniper-lib/threejs/eventSystem/PointerEventTypes";
import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { objGraph } from "../objects";
import type { BaseCursor } from "./BaseCursor";
import { CursorXRMouse } from "./CursorXRMouse";
import type { IPointer, PointerType } from "./IPointer";
import { getRayTarget } from "./RayTarget";

const MAX_DRAG_DISTANCE = 5;

export abstract class BasePointer
    implements IPointer {
    private _cursor: BaseCursor;
    private _canMoveView = false;
    private _enabled = false;

    private canClick = false;
    protected _buttons = MouseButtons.None;
    private lastButtons = 0;
    protected moveDistance = 0;
    private _dragging = false;
    private wasDragging = false;
    private dragDistance = 0;

    isActive = false;
    origin = new THREE.Vector3();
    direction = new THREE.Vector3();

    curHit: THREE.Intersection = null;
    hoveredHit: THREE.Intersection = null;
    private _pressedHit: THREE.Intersection = null;
    draggedHit: THREE.Intersection = null;


    constructor(
        public readonly type: PointerType,
        public name: PointerName,
        protected readonly env: BaseEnvironment,
        cursor: BaseCursor) {

        this._cursor = cursor;
        this.enabled = false;
        this.canMoveView = false;
    }

    get pressedHit(): THREE.Intersection {
        return this._pressedHit;
    }

    set pressedHit(v: THREE.Intersection) {
        this._pressedHit = v;
        const target = getRayTarget(v);
        if (target && target.draggable && !target.clickable) {
            this.onDragStart();
        }
    }

    get canMoveView() {
        return this._canMoveView;
    }

    set canMoveView(v) {
        this._canMoveView = v;
    }

    vibrate(): void {
        // nothing to do in the base case.
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
                oldParent.remove(oldCursor.object);
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

    get enabled() {
        return this._enabled;
    }

    set enabled(v) {
        this._enabled = v;
        if (this.cursor) {
            this.cursor.visible = v;
        }
    }

    get buttons() {
        return this._buttons;
    }

    get dragging() {
        return this._dragging;
    }

    set dragging(v) {
        this._dragging = v;
        this.dragDistance = 0;
    }

    get needsUpdate() {
        return this.enabled
            && this.isActive;
    }

    protected setEventState(type: SourcePointerEventTypes): void {
        this.env.eventSystem.checkPointer(this, type);
    }

    update(): void {
        if (this.needsUpdate) {
            this.onUpdate();
            this.lastButtons = this.buttons;
            this.wasDragging = this.dragging;
        }
    }

    protected abstract onUpdate(): void;

    updateCursor(avatarHeadPos: THREE.Vector3, curHit: THREE.Intersection, defaultDistance: number) {
        if (this.cursor) {
            this.cursor.update(avatarHeadPos, curHit, defaultDistance, this.canMoveView, this.origin, this.direction, this.buttons, this.dragging);
        }
    }

    protected onPointerDown(): void {
        this.dragging = false;
        this.canClick = true;
        this.env.avatar.setMode(this);
        this.setEventState("down");
    }

    protected onPointerMove() {
        this.setEventState("move");
        if (this.buttons !== MouseButtons.None) {
            const target = getRayTarget(this.pressedHit);
            const canDrag = !target || target.draggable;
            if (canDrag) {
                if (this.buttons === this.lastButtons) {
                    this.dragDistance += this.moveDistance;
                    if (this.dragDistance > MAX_DRAG_DISTANCE) {
                        this.onDragStart();
                    }
                }
                else if (this.dragging) {
                    this.dragging = false;
                    this.setEventState("dragcancel");
                }
            }
        }
    }

    private onDragStart() {
        this.dragging = true;

        if (!this.wasDragging) {
            this.setEventState("dragstart");
        }

        this.canClick = false;
        this.setEventState("drag");
    }

    protected onPointerUp() {
        if (this.canClick) {
            const curButtons = this.buttons;
            this._buttons = this.lastButtons;
            this.setEventState("click");
            this._buttons = curButtons;
        }

        this.setEventState("up");

        this.dragging = false;
        if (this.wasDragging) {
            this.setEventState("dragend");
        }
    }

    abstract isPressed(button: VirtualButtons): boolean;
}

