import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";

export class PointerMouse extends BaseScreenPointerSinglePoint {
    allowPointerLock: boolean = false;

    constructor(env: BaseEnvironment) {
        super("mouse", PointerName.Mouse, env);

        this.element.addEventListener("wheel", (evt: WheelEvent) => {
            evt.preventDefault();
            this.env.avatar.zoom(-evt.deltaY * 0.5);
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
