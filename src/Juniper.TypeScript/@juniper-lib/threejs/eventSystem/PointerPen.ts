import { PointerName } from "@juniper-lib/event-system/PointerName";
import { BaseScreenPointer } from "./BaseScreenPointer";
import { CursorXRMouse } from "./CursorXRMouse";
import type { EventSystem } from "./EventSystem";

export class PointerPen extends BaseScreenPointer {
    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {

        const onPrep = (evt: PointerEvent) => {
            if (evt.pointerType === "pen"
                && this.id == null) {
                this.id = evt.pointerId;
            }
        };

        const unPrep = (evt: PointerEvent) => {
            if (evt.pointerType === "pen"
                && this.id != null) {
                this.id = null;
            }
        };

        const element = renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);

        super("pen", PointerName.Pen, evtSys, renderer, camera, new CursorXRMouse(renderer));

        element.addEventListener("pointerup", unPrep);
        element.addEventListener("pointercancel", unPrep);

        this.canMoveView = true;

        Object.seal(this);
    }
}
