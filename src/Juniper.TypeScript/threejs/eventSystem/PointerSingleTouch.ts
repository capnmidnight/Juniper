import { PointerEventTypes } from "juniper-dom/eventSystem/PointerEventTypes";
import { PointerName } from "juniper-dom/eventSystem/PointerName";
import { arrayScan } from "juniper-tslib";
import { BaseScreenPointer } from "./BaseScreenPointer";
import type { EventSystem } from "./EventSystem";
import type { PointerMultiTouch } from "./PointerMultiTouch";

const touches = new Array<PointerSingleTouch>();

const onPrePointerDown = (evt: PointerEvent) => {
    if (evt.pointerType === "touch") {
        const pressCount = countPresses();

        let touch = arrayScan(touches, (t) => t.id === evt.pointerId);
        if (!touch) {
            touch = arrayScan(touches, (t) => t.id == null);
            touch.id = evt.pointerId;
            touch.state.buttons = 1 << pressCount;
        }
    }
};

const onPostPointerUp = (evt: PointerEvent) => {
    if (evt.pointerType === "touch") {
        let touch = arrayScan(touches, (t) => t.id === evt.pointerId);
        if (touch) {
            touch.id = null;
        }
    }
};

export function countPresses() {
    let count = 0;
    for (const touch of touches) {
        if (touch.isTracking) {
            ++count;
        }
    }
    return count;
}

const pointerNames = new Map<number, PointerName>([
    [0, PointerName.Touch0],
    [1, PointerName.Touch1],
    [2, PointerName.Touch2],
    [3, PointerName.Touch3],
    [4, PointerName.Touch4],
    [5, PointerName.Touch5],
    [6, PointerName.Touch6],
    [7, PointerName.Touch7],
    [8, PointerName.Touch8],
    [9, PointerName.Touch9],
    [10, PointerName.Touch10]
]);

export class PointerSingleTouch extends BaseScreenPointer {
    private readonly parent: PointerMultiTouch = null;

    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, idx: number, camera: THREE.PerspectiveCamera, parent?: PointerMultiTouch) {
        if (touches.length === 0) {
            renderer.domElement.addEventListener("pointerdown", onPrePointerDown);
        }
        super("touch", pointerNames.get(idx), evtSys, renderer, camera, null);
        if (touches.length === 0) {
            renderer.domElement.addEventListener("pointerup", onPostPointerUp);
        }
        this.parent = parent;
        this.canMoveView = true;
        Object.seal(this);
        touches.push(this);
    }

    override setEventState(type: PointerEventTypes): void {
        super.setEventState(type);
        if (this.parent) {
            if (type === "down") {
                this.parent.onDown();
            }
            else if (type === "up") {
                this.parent.onUp();
            }
            else if (type === "move") {
                this.parent.onMove();
            }
        }
    }

    override readEvent(evt: PointerEvent) {
        if (this.checkEvent(evt)) {
            const lastButtons = this.state.buttons;
            super.readEvent(evt);
            this.state.buttons = lastButtons;
        }
    }
}
