import { isModifierless } from "@juniper-lib/dom/isModifierless";
import { FlickEvent } from "@juniper-lib/event-system/FlickEvent";
import { ObjectMovedEvent } from "@juniper-lib/event-system/ObjectMovedEvent";
import { PointerEventTypes } from "@juniper-lib/event-system/PointerEventTypes";
import { VirtualButtons } from "@juniper-lib/event-system/VirtualButtons";
import { arrayClear, assertNever, isDefined, TypedEventBase } from "@juniper-lib/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { FOREGROUND } from "../layers";
import { objGraph } from "../objects";
import { EventSystemEvent } from "./EventSystemEvent";
import { isClickable, isDraggable, isInteractiveHit, isObjVisible } from "./InteractiveObject3D";
import type { IPointer } from "./IPointer";
import { PointerHand } from "./PointerHand";
import { PointerMouse } from "./PointerMouse";
import { PointerMultiTouch } from "./PointerMultiTouch";
import { PointerPen } from "./PointerPen";
import { resolveObj } from "./resolveObj";

interface EventSystemEvents {
    move: EventSystemEvent<"move">;
    enter: EventSystemEvent<"enter">;
    exit: EventSystemEvent<"exit">;
    up: EventSystemEvent<"up">;
    down: EventSystemEvent<"down">;
    click: EventSystemEvent<"click">;
    dragstart: EventSystemEvent<"dragstart">;
    drag: EventSystemEvent<"drag">;
    dragend: EventSystemEvent<"dragend">;
    objectMoved: ObjectMovedEvent;
    flick: FlickEvent;
}

function correctHit(hit: THREE.Intersection, pointer: IPointer) {
    if (isDefined(hit)) {
        hit.point
            .copy(pointer.direction)
            .multiplyScalar(hit.distance)
            .add(pointer.origin);
    }
}

export class EventSystem extends TypedEventBase<EventSystemEvents> {
    private readonly raycaster = new THREE.Raycaster();

    private readonly mouse: PointerMouse;
    private readonly pen: PointerPen;
    private readonly touches: PointerMultiTouch;
    readonly hands = new Array<PointerHand>();

    private readonly pointers: IPointer[];
    private readonly localPointerMovedEvt = new ObjectMovedEvent();

    private readonly hits = new Array<THREE.Intersection>();

    private infoPressed = false;
    private menuPressed = false;

    private readonly pointerEvents = new Map<IPointer, Map<string, EventSystemEvent<string>>>();

    constructor(
        private readonly env: BaseEnvironment<unknown>) {
        super();

        this.raycaster.camera = this.env.camera;
        this.raycaster.layers.set(FOREGROUND);

        this.mouse = new PointerMouse(this, this.env.renderer, this.env.camera);
        this.pen = new PointerPen(this, this.env.renderer, this.env.camera);
        this.touches = new PointerMultiTouch(this, this.env.renderer, this.env.camera);

        for (let i = 0; i < 2; ++i) {
            this.hands[i] = new PointerHand(this, this.env.renderer, i);
        }

        this.pointers = [
            this.mouse,
            this.pen,
            this.touches,
            ...this.hands
        ];

        for (const pointer of this.pointers) {
            if (pointer.cursor) {
                objGraph(this.env.stage, pointer.cursor);
            }
        }

        window.addEventListener("keydown", (evt) => {
            const ok = isModifierless(evt);
            if (evt.key === "/") this.infoPressed = ok;
            if (evt.key === "ContextMenu") this.menuPressed = ok;
        });

        window.addEventListener("keyup", (evt) => {
            if (evt.key === "/") this.infoPressed = false;
            if (evt.key === "ContextMenu") this.menuPressed = false;
        });

        this.env.avatar.evtSys = this;

        this.checkXRMouse();
    }

    onFlick(direction: number) {
        this.env.avatar.onFlick(direction);
    }

    onConnected(_hand: PointerHand) {
        this.checkXRMouse();
    }

    onDisconnected(_hand: PointerHand) {
        this.checkXRMouse();
    }

    private checkXRMouse() {
        let count = 0;
        for (const hand of this.hands.values()) {
            if (hand.enabled) {
                ++count;
            }
        }

        const enableScreenPointers = count === 0;
        this.mouse.enabled = enableScreenPointers;
        this.pen.enabled = enableScreenPointers;
        this.touches.enabled = enableScreenPointers;
    }

    checkPointer(pointer: IPointer, eventType: PointerEventTypes) {
        pointer.isActive = true;

        this.fireRay(pointer);

        const { curHit, hoveredHit, pressedHit, draggedHit } = pointer;

        const curObj = resolveObj(curHit);
        const hoveredObj = resolveObj(hoveredHit);
        const pressedObj = resolveObj(pressedHit);
        const draggedObj = resolveObj(draggedHit);

        if (eventType === "move" || eventType === "drag") {
            correctHit(hoveredHit, pointer);
            correctHit(pressedHit, pointer);
            correctHit(draggedHit, pointer);
        }

        switch (eventType) {
            case "move":
                {
                    const moveEvt = this.getEvent(pointer, "move", curHit);
                    this.env.avatar.onMove(moveEvt);
                    this.env.cameraControl.onMove(moveEvt);

                    if (isDefined(draggedHit)) {
                        draggedObj.dispatchEvent(moveEvt.to3(draggedHit));
                    }
                    else if (isDefined(pressedHit)) {
                        pressedObj.dispatchEvent(moveEvt.to3(pressedHit));
                    }
                    else if (pointer.state.buttons === 0) {
                        this.checkExit(curHit, hoveredHit, pointer);
                        this.checkEnter(curHit, hoveredHit, pointer);
                        if (curObj) {
                            curObj.dispatchEvent(moveEvt.to3(curHit));
                        }
                    }

                    this.localPointerMovedEvt.name = pointer.name;
                    this.localPointerMovedEvt.set(
                        pointer.origin.x, pointer.origin.y, pointer.origin.z,
                        pointer.direction.x, pointer.direction.y, pointer.direction.z,
                        0, 1, 0);

                    this.dispatchEvent(this.localPointerMovedEvt);
                }
                break;

            case "down":
                {
                    const downEvt = this.getEvent(pointer, "down", curHit);
                    this.env.avatar.onDown(downEvt);

                    if (isClickable(hoveredHit)
                        || isDraggable(hoveredHit)) {
                        pointer.pressedHit = hoveredHit;

                        hoveredObj.dispatchEvent(downEvt.to3(hoveredHit));
                    }
                }
                break;

            case "up":
                {
                    const upEvt = this.getEvent(pointer, "up", curHit);
                    this.dispatchEvent(upEvt);

                    if (pointer.state.buttons === 0) {
                        if (isDefined(pressedHit)) {
                            pointer.pressedHit = null;
                            pressedObj.dispatchEvent(upEvt.to3(pressedHit));
                        }

                        this.checkExit(curHit, hoveredHit, pointer);
                        this.checkEnter(curHit, hoveredHit, pointer);
                    }
                }
                break;

            case "click":
                {
                    const clickEvt = this.getEvent(pointer, "click", curHit);
                    this.dispatchEvent(clickEvt);

                    if (isClickable(curHit)) {
                        pointer.vibrate();
                        curObj.dispatchEvent(clickEvt.to3(curHit));
                    }
                }
                break;

            case "dragstart":
                {
                    const dragStartEvt = this.getEvent(pointer, "dragstart", pressedHit || curHit);
                    this.dispatchEvent(dragStartEvt);

                    if (isDefined(pressedHit)) {
                        pointer.draggedHit = pressedHit;
                        pressedObj.dispatchEvent(dragStartEvt.to3(pressedHit));
                    }
                }
                break;

            case "drag":
                {
                    const dragEvt = this.getEvent(pointer, "drag", draggedHit || curHit);
                    this.dispatchEvent(dragEvt);

                    if (isDefined(draggedHit)) {
                        draggedObj.dispatchEvent(dragEvt.to3(draggedHit));
                    }
                }
                break;

            case "dragcancel":
                {
                    const dragCancelEvt = this.getEvent(pointer, "dragcancel", draggedHit || curHit);
                    this.dispatchEvent(dragCancelEvt);

                    if (isDefined(draggedHit)) {
                        pointer.draggedHit = null;
                        draggedObj.dispatchEvent(dragCancelEvt.to3(draggedHit));
                    }
                }
                break;

            case "dragend":
                {
                    const dragEndEvt = this.getEvent(pointer, "dragend", draggedHit || curHit);
                    this.dispatchEvent(dragEndEvt);

                    if (isDefined(draggedHit)) {
                        pointer.draggedHit = null;
                        draggedObj.dispatchEvent(dragEndEvt.to3(draggedHit));
                    }
                }
                break;

            default:
                assertNever(eventType);
        }

        pointer.updateCursor(
            this.env.avatar.worldPos,
            draggedHit || pressedHit || hoveredHit || curHit,
            2);
    }

    private getEvent<T extends string>(pointer: IPointer, type: T, hit: THREE.Intersection): EventSystemEvent<T> {
        if (!this.pointerEvents.has(pointer)) {
            const evts = new Map<string, EventSystemEvent<string>>([
                ["move", new EventSystemEvent("move", pointer)],
                ["enter", new EventSystemEvent("enter", pointer)],
                ["exit", new EventSystemEvent("exit", pointer)],
                ["up", new EventSystemEvent("up", pointer)],
                ["down", new EventSystemEvent("down", pointer)],
                ["click", new EventSystemEvent("click", pointer)],
                ["dragstart", new EventSystemEvent("dragstart", pointer)],
                ["drag", new EventSystemEvent("drag", pointer)],
                ["dragend", new EventSystemEvent("dragend", pointer)]
            ]);
            this.pointerEvents.set(pointer, evts);
        }

        const pointerEvents = this.pointerEvents.get(pointer);
        const evt = pointerEvents.get(type) as EventSystemEvent<T>
        evt.hit = hit;
        return evt;
    }

    private checkExit(curHit: THREE.Intersection, hoveredHit: THREE.Intersection, pointer: IPointer) {
        const curObj = resolveObj(curHit);
        const hoveredObj = resolveObj(hoveredHit);
        if (curObj !== hoveredObj && isDefined(hoveredObj)) {
            pointer.hoveredHit = null;

            const exitEvt = this.getEvent(pointer, "exit", hoveredHit);
            this.dispatchEvent(exitEvt);

            hoveredObj.dispatchEvent(exitEvt.to3(hoveredHit));
        }
    }

    private checkEnter(curHit: THREE.Intersection, hoveredHit: THREE.Intersection, pointer: IPointer) {
        const curObj = resolveObj(curHit);
        const hoveredObj = resolveObj(hoveredHit);
        if (curObj !== hoveredObj && isDefined(curHit)) {
            pointer.hoveredHit = curHit;

            const enterEvt = this.getEvent(pointer, "enter", curHit);
            this.dispatchEvent(enterEvt);

            curObj.dispatchEvent(enterEvt.to3(curHit));
        }
    }

    refreshCursors() {
        for (const pointer of this.pointers) {
            if (pointer.cursor) {
                pointer.cursor = this.env.cursor3D.clone();
            }
        }
    }

    fireRay(pointer: IPointer): void {
        arrayClear(this.hits);

        this.raycaster.ray.origin.copy(pointer.origin);
        this.raycaster.ray.direction.copy(pointer.direction);
        this.raycaster.intersectObject(this.env.scene, true, this.hits);

        pointer.curHit = null;
        let minDist = Number.MAX_VALUE;
        for (const hit of this.hits) {
            if (isInteractiveHit(hit)
                && isObjVisible(hit)
                && hit.distance < minDist) {
                pointer.curHit = hit;
                minDist = hit.distance;
            }
        }

        if (pointer.curHit
            && pointer.hoveredHit
            && pointer.curHit.object === pointer.hoveredHit.object) {
            pointer.hoveredHit = pointer.curHit;
        }
    }

    update() {
        for (const pointer of this.pointers) {
            if (pointer.needsUpdate) {
                pointer.update();
            }
        }
    }

    recheckPointers() {
        for (const pointer of this.pointers) {
            if (pointer.needsUpdate) {
                pointer.recheck();
            }
        }
    }

    isPressed(button: VirtualButtons): boolean {
        for (const pointer of this.pointers) {
            if (pointer.isPressed(button)) {
                return true;
            }
        }

        return button === VirtualButtons.Menu && this.menuPressed
            || button === VirtualButtons.Info && this.infoPressed;
    }
}