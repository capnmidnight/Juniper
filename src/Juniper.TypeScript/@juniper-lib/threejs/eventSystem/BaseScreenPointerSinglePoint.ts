import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointer } from "./BaseScreenPointer";
import { CursorXRMouse } from "./CursorXRMouse";
import type { EventSystem } from "./EventSystem";
import { PointerType } from "./IPointer";

export abstract class BaseScreenPointerSinglePoint extends BaseScreenPointer {
    constructor(type: PointerType, name: PointerName, evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {

        const onPrep = (evt: PointerEvent) => {
            if (evt.pointerType === type
                && this.id == null) {
                this.id = evt.pointerId;
            }
        };

        const unPrep = (evt: PointerEvent) => {
            if (evt.pointerType === type
                && this.id != null) {
                this.id = null;
            }
        };

        const element = renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);

        super(type, name, evtSys, renderer, camera, new CursorXRMouse(renderer));

        element.addEventListener("pointerup", unPrep);
        element.addEventListener("pointercancel", unPrep);

        this.canMoveView = true;
    }

    protected onReadEvent(evt: PointerEvent) {
        this.state.buttons = evt.buttons;
        this.state.position.set(evt.offsetX, -evt.offsetY);
        this.state.motion.set(evt.movementX, -evt.movementY);
    }
}
