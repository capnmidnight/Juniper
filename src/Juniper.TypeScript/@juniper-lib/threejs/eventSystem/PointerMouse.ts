import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
import type { EventSystem } from "./EventSystem";

export class PointerMouse extends BaseScreenPointerSinglePoint {
    allowPointerLock: boolean = false;

    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {
        super("mouse", PointerName.Mouse, evtSys, renderer, camera);

        this.element.addEventListener("wheel", (evt: WheelEvent) => {
            evt.preventDefault();
            this.state.dz = -evt.deltaY * 0.5;
        }, { passive: false });

        this.element.addEventListener("contextmenu", (evt: Event) => {
            evt.preventDefault();
        });

        this.element.addEventListener("pointerdown", () => {
            if (this.allowPointerLock
                && !this.isPointerLocked) {
                this.lockPointer();
            }
        });

        Object.seal(this);
    }

    lockPointer() {
        this.element.requestPointerLock();
    }

    unlockPointer() {
        document.exitPointerLock();
    }

    get isPointerLocked() {
        return document.pointerLockElement != null;
    }
}
