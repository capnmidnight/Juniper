import { MouseButtons } from "juniper-dom/eventSystem/MouseButton";
import { PointerEventTypes } from "juniper-dom/eventSystem/PointerEventTypes";
import { PointerName } from "juniper-dom/eventSystem/PointerName";
import { PointerState } from "juniper-dom/eventSystem/PointerState";
import { VirtualButtons } from "juniper-dom/eventSystem/VirtualButtons";
import { isNullOrUndefined } from "juniper-tslib";
import { objGraph } from "../objects";
import type { BaseCursor } from "./BaseCursor";
import { CursorXRMouse } from "./CursorXRMouse";
import type { EventSystem } from "./EventSystem";
import { isClickable, isDraggable } from "./InteractiveObject3D";
import type { IPointer, PointerType } from "./IPointer";

const MAX_DRAG_DISTANCE = 5;

export abstract class BasePointer
    implements IPointer {
    private _cursor: BaseCursor;
    private _canMoveView = false;
    private _enabled = false;

    isActive = false;
    movementDragThreshold = MAX_DRAG_DISTANCE;
    state = new PointerState();
    lastState: PointerState = null;
    origin = new THREE.Vector3();
    direction = new THREE.Vector3();

    curHit: THREE.Intersection = null;
    hoveredHit: THREE.Intersection = null;
    private _pressedHit: THREE.Intersection = null;
    draggedHit: THREE.Intersection = null;


    constructor(
        public readonly type: PointerType,
        public name: PointerName,
        protected readonly evtSys: EventSystem,
        cursor: BaseCursor) {
        //super();

        this._cursor = cursor;
        this.enabled = false;
        this.canMoveView = false;
    }

    get pressedHit(): THREE.Intersection {
        return this._pressedHit;
    }

    set pressedHit(v: THREE.Intersection) {
        this._pressedHit = v;
        if (isDraggable(v) && !isClickable(v)) {
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
                objGraph
                oldParent.remove(oldCursor.object);
            }

            if (newCursor) {
                newCursor.object.name = oldName;

                if (oldCursor instanceof CursorXRMouse) {
                    oldCursor.cursor = newCursor;
                    if (oldParent) {
                        oldParent.add(oldCursor.object);
                    }
                }
                else {
                    this._cursor = newCursor;
                    if (oldCursor) {
                        if (oldParent) {
                            oldParent.add(newCursor.object);
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

    get needsUpdate() {
        return this.enabled
            && this.isActive;
    }

    recheck(): void { }

    protected setEventState(type: PointerEventTypes): void {
        this.evtSys.checkPointer(this, type);
    }

    abstract update(): void;

    updateCursor(avatarHeadPos: THREE.Vector3, curHit: THREE.Intersection, defaultDistance: number) {
        if (this.cursor) {
            this.cursor.update(avatarHeadPos, curHit, defaultDistance, this.canMoveView, this.state, this.origin, this.direction);
        }
    }

    protected lastStateUpdate(updater: () => void): void {
        if (this.lastState) {
            this.lastState.copy(this.state);
            updater();
            this.state.dragDistance = this.lastState.dragDistance;
        }
        else {
            updater();
            this.lastState = new PointerState();
            this.lastState.copy(this.state);
        }
    }

    protected onZoom(dz: number): void {
        this.lastStateUpdate(() => this.state.dz = dz);
        this.setEventState("move");
    }

    protected onPointerDown(): void {
        this.state.dragging = false;
        this.state.canClick = true;
        this.setEventState("down");
    }

    protected onPointerMove() {
        this.setEventState("move");

        if (this.state.buttons !== MouseButtons.None) {
            const canDrag = isNullOrUndefined(this.pressedHit)
                || isDraggable(this.pressedHit);
            if (canDrag) {
                if (this.lastState.buttons === this.state.buttons) {
                    this.state.dragDistance += this.state.moveDistance;
                    if (this.state.dragDistance > this.movementDragThreshold) {
                        this.onDragStart();
                    }
                }
                else if (this.state.dragging) {
                    this.state.dragging = false;
                    this.setEventState("dragcancel");
                }
            }
        }
    }

    private onDragStart() {
        const wasDragging = this.state.dragging;
        this.state.dragging = true;

        if (!wasDragging) {
            this.setEventState("dragstart");
        }

        this.state.canClick = false;
        this.setEventState("drag");
    }

    protected onPointerUp() {
        if (this.state.canClick) {
            const lastButtons = this.state.buttons;
            this.state.buttons = this.lastState.buttons;
            this.setEventState("click");
            this.state.buttons = lastButtons;
        }

        this.setEventState("up");

        this.state.dragDistance = 0;
        const wasDragging = this.state.dragging;
        this.state.dragging = false;
        if (wasDragging) {
            this.setEventState("dragend");
        }
    }

    abstract isPressed(button: VirtualButtons): boolean;
}
