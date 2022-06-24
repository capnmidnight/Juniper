import { isModifierless } from "@juniper-lib/dom/evts";
import { ObjectMovedEvent } from "@juniper-lib/threejs/eventSystem/ObjectMovedEvent";
import { PointerEventTypes, SourcePointerEventTypes } from "@juniper-lib/threejs/eventSystem/PointerEventTypes";
import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import { arrayClear, arrayScan, assertNever, isDefined, TypedEventBase } from "@juniper-lib/tslib";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { FOREGROUND } from "../layers";
import { objGraph } from "../objects";
import { EventSystemEvent, EventSystemEvents } from "./EventSystemEvent";
import type { IPointer } from "./IPointer";
import { PointerHand } from "./PointerHand";
import { PointerMouse } from "./PointerMouse";
import { PointerMultiTouch } from "./PointerMultiTouch";
import { PointerPen } from "./PointerPen";
import { getRayTarget } from "./RayTarget";

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

    public readonly mouse: PointerMouse;
    private readonly pen: PointerPen;
    private readonly touches: PointerMultiTouch;
    readonly hands = new Array<PointerHand>();

    private readonly pointers: IPointer[];
    private readonly pointerMovedEvts = new Map<PointerName, ObjectMovedEvent>();

    private readonly hits = new Array<THREE.Intersection>();

    private infoPressed = false;
    private menuPressed = false;

    private readonly pointerEvents = new Map<IPointer, Map<string, EventSystemEvent<PointerEventTypes>>>();

    constructor(
        private readonly env: BaseEnvironment<unknown>) {
        super();

        this.raycaster.camera = this.env.camera;
        this.raycaster.layers.set(FOREGROUND);

        this.mouse = new PointerMouse(this.env);
        this.pen = new PointerPen(this.env);
        this.touches = new PointerMultiTouch(this.env);

        for (let i = 0; i < 2; ++i) {
            this.hands[i] = new PointerHand(this.env, i);
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

    checkPointer(pointer: IPointer, eventType: SourcePointerEventTypes) {
        this.fireRay(pointer);

        const { curHit, hoveredHit, pressedHit, draggedHit } = pointer;

        const curTarget = getRayTarget(curHit);
        const hovTarget = getRayTarget(hoveredHit);
        const prsTarget = getRayTarget(pressedHit);
        const drgTarget = getRayTarget(draggedHit);

        if (eventType === "move" || eventType === "drag") {
            correctHit(hoveredHit, pointer);
            correctHit(pressedHit, pointer);
            correctHit(draggedHit, pointer);
        }

        switch (eventType) {
            case "move":
                {
                    const moveEvt = this.getEvent(pointer, "move", curHit);

                    if (isDefined(drgTarget)) {
                        drgTarget.dispatchEvent(moveEvt);
                    }
                    else if (isDefined(prsTarget)) {
                        prsTarget.dispatchEvent(moveEvt);
                    }
                    else if (pointer.buttons === 0) {
                        this.checkExit(curHit, hoveredHit, pointer);
                        this.checkEnter(curHit, hoveredHit, pointer);
                        if (curTarget) {
                            curTarget.dispatchEvent(moveEvt);
                        }
                    }

                    let evt = this.pointerMovedEvts.get(pointer.name);
                    if (!evt) {
                        this.pointerMovedEvts.set(pointer.name, evt = new ObjectMovedEvent(pointer.name));
                    }

                    evt.set(
                        pointer.origin.x, pointer.origin.y, pointer.origin.z,
                        pointer.direction.x, pointer.direction.y, pointer.direction.z,
                        0, 1, 0);

                    this.dispatchEvent(evt);
                }
                break;

            case "down":
                {
                    const downEvt = this.getEvent(pointer, "down", curHit);

                    if (hovTarget &&
                        (hovTarget.clickable
                            || hovTarget.draggable)) {
                        pointer.pressedHit = hoveredHit;

                        hovTarget.dispatchEvent(downEvt);
                    }
                }
                break;

            case "up":
                {
                    const upEvt = this.getEvent(pointer, "up", curHit);
                    this.dispatchEvent(upEvt);

                    if (pointer.buttons === 0) {
                        if (isDefined(pressedHit)) {
                            pointer.pressedHit = null;
                            prsTarget.dispatchEvent(upEvt);
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

                    if (curTarget && curTarget.clickable) {
                        pointer.vibrate();
                        curTarget.dispatchEvent(clickEvt);
                    }
                }
                break;

            case "dragstart":
                {
                    const dragStartEvt = this.getEvent(pointer, "dragstart", pressedHit, curHit);
                    this.dispatchEvent(dragStartEvt);

                    if (isDefined(prsTarget)) {
                        pointer.draggedHit = pressedHit;
                        prsTarget.dispatchEvent(dragStartEvt);
                    }
                }
                break;

            case "drag":
                {
                    const dragEvt = this.getEvent(pointer, "drag", draggedHit, curHit);
                    this.dispatchEvent(dragEvt);

                    if (isDefined(drgTarget)) {
                        drgTarget.dispatchEvent(dragEvt);
                    }
                }
                break;

            case "dragcancel":
                {
                    const dragCancelEvt = this.getEvent(pointer, "dragcancel", draggedHit, curHit);
                    this.dispatchEvent(dragCancelEvt);

                    if (isDefined(drgTarget)) {
                        pointer.draggedHit = null;
                        drgTarget.dispatchEvent(dragCancelEvt);
                    }
                }
                break;

            case "dragend":
                {
                    const dragEndEvt = this.getEvent(pointer, "dragend", draggedHit, curHit);
                    this.dispatchEvent(dragEndEvt);

                    if (isDefined(drgTarget)) {
                        pointer.draggedHit = null;
                        drgTarget.dispatchEvent(dragEndEvt);
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

    private getEvent<T extends PointerEventTypes>(pointer: IPointer, type: T, ...hits: THREE.Intersection[]): EventSystemEvent<T> {
        if (!this.pointerEvents.has(pointer)) {
            this.pointerEvents.set(pointer, new Map<PointerEventTypes, EventSystemEvent<PointerEventTypes>>());
        }

        const pointerEvents = this.pointerEvents.get(pointer);
        if (!pointerEvents.has(type)) {
            pointerEvents.set(type, new EventSystemEvent(type, pointer));
        }

        const evt = pointerEvents.get(type) as EventSystemEvent<T>
        if (hits.length > 0) {
            evt.hit = arrayScan(hits, isDefined);
            const lastHit = arrayScan(hits, (h) => isDefined(h) && h !== evt.hit);
            if (isDefined(lastHit)) {
                evt.hit.uv = lastHit.uv;
            }
        }
        return evt;
    }

    private checkExit(curHit: THREE.Intersection, hoveredHit: THREE.Intersection, pointer: IPointer) {
        const curTarget = getRayTarget(curHit);
        const hoveredTarget = getRayTarget(hoveredHit);
        if (curTarget !== hoveredTarget && isDefined(hoveredTarget)) {
            pointer.hoveredHit = null;

            const exitEvt = this.getEvent(pointer, "exit", hoveredHit);
            this.dispatchEvent(exitEvt);
            hoveredTarget.dispatchEvent(exitEvt);
        }
    }

    private checkEnter(curHit: THREE.Intersection, hoveredHit: THREE.Intersection, pointer: IPointer) {
        const curTarget = getRayTarget(curHit);
        const hoveredTarget = getRayTarget(hoveredHit);
        if (curTarget !== hoveredTarget && isDefined(curHit)) {
            pointer.hoveredHit = curHit;

            const enterEvt = this.getEvent(pointer, "enter", curHit);
            this.dispatchEvent(enterEvt);
            curTarget.dispatchEvent(enterEvt);
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
            const rayTarget = getRayTarget(hit);
            if (rayTarget
                && rayTarget.object.visible
                && hit.distance < minDist) {
                pointer.curHit = hit;
                minDist = hit.distance;
            }
        }

        if (pointer.curHit) {
            if (pointer.hoveredHit
                && pointer.curHit.object === pointer.hoveredHit.object) {
                pointer.hoveredHit = pointer.curHit;
            }

            if (pointer.draggedHit
                && pointer.curHit.object === pointer.draggedHit.object) {
                pointer.draggedHit = pointer.curHit;
            }
        }
    }

    update() {
        for (const pointer of this.pointers) {
            if (pointer.needsUpdate) {
                pointer.update();
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