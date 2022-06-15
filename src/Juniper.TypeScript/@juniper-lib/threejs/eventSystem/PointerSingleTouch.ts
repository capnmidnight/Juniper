import { arrayScan } from "@juniper-lib/tslib";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointer } from "./BaseScreenPointer";
import type { EventSystem } from "./EventSystem";

const touches = new Array<PointerSingleTouch>();

const onPrePointerDown = (evt: PointerEvent) => {
    if (evt.pointerType === "touch") {
        let pressCount = 0;
        for (const touch of touches) {
            if (touch.isTracking) {
                ++pressCount;
            }
        }

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

export class PointerSingleTouch extends BaseScreenPointer {
    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, name: PointerName, camera: THREE.PerspectiveCamera) {
        if (touches.length === 0) {
            renderer.domElement.addEventListener("pointerdown", onPrePointerDown);
        }
        super("touch", name, evtSys, renderer, camera, null);
        if (touches.length === 0) {
            renderer.domElement.addEventListener("pointerup", onPostPointerUp);
        }
        this.canMoveView = true;
        Object.seal(this);
        touches.push(this);
    }

    protected override readEvent(evt: PointerEvent) {
        if (this.checkEvent(evt)) {
            const lastButtons = this.state.buttons;
            super.readEvent(evt);
            this.state.buttons = lastButtons;
        }
    }
}
