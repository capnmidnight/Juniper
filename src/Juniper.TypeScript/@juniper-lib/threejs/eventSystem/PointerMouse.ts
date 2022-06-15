import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointer } from "./BaseScreenPointer";
import { CursorXRMouse } from "./CursorXRMouse";
import type { EventSystem } from "./EventSystem";

export class PointerMouse extends BaseScreenPointer {
    allowPointerLock: boolean = false;

    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {

        const onPrep = (evt: PointerEvent) => {
            if (evt.pointerType === "mouse"
                && this.id == null) {
                this.id = evt.pointerId;
            }
        };

        const unPrep = (evt: PointerEvent) => {
            if (evt.pointerType === "mouse"
                && this.id != null) {
                this.id = null;
            }
        };

        const element = renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);

        super("mouse", PointerName.Mouse, evtSys, renderer, camera, new CursorXRMouse(renderer));

        this.element.addEventListener("wheel", (evt: WheelEvent) => {
            evt.preventDefault();
            const dz = -evt.deltaY * 0.5;
            this.onZoom(dz);
        }, { passive: false });

        this.element.addEventListener("contextmenu", (evt: Event) => {
            evt.preventDefault();
        });

        this.element.addEventListener("pointerup", unPrep);
        this.element.addEventListener("pointercancel", unPrep);

        this.element.addEventListener("pointerdown", () => {
            if (this.allowPointerLock
                && !this.isPointerLocked) {
                this.lockPointer();
            }
        });

        this.canMoveView = true;

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
