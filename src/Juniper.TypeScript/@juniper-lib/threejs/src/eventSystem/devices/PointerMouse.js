import { isModifierless } from "@juniper-lib/dom/evts";
import { PointerID } from "../Pointers";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
import { VirtualButton } from "./VirtualButton";
export class PointerMouse extends BaseScreenPointerSinglePoint {
    constructor(env) {
        super("mouse", PointerID.Mouse, env);
        this.allowPointerLock = false;
        this.dz = 0;
        this.keyMap = new Map([
            ["`", VirtualButton.Info],
            ["ContextMenu", VirtualButton.Menu]
        ]);
        this.element.addEventListener("wheel", (evt) => {
            evt.preventDefault();
            this.dz += -evt.deltaY * 0.1;
        }, { passive: false });
        this.element.addEventListener("contextmenu", (evt) => {
            evt.preventDefault();
        });
        this.element.addEventListener("pointerdown", (evt) => {
            if (this.onCheckEvent(evt)) {
                if (this.allowPointerLock
                    && !this.isPointerLocked) {
                    this.lockPointer();
                }
                else if (!this.allowPointerLock
                    && !this.isPointerCaptured) {
                    this.capturePointer();
                }
            }
        });
        this.element.addEventListener("pointerup", (evt) => {
            if (this.onCheckEvent(evt)) {
                if (this.allowPointerLock
                    && this.isPointerLocked) {
                    this.unlockPointer();
                }
                else if (!this.allowPointerLock
                    && this.isPointerCaptured) {
                    this.releaseCapture();
                }
            }
        }, true);
        document.addEventListener("pointerlockchange", () => {
            this.cursor.visible = true;
        });
        window.addEventListener("keydown", (evt) => {
            if (this._isActive && this.keyMap.has(evt.key)) {
                this.setButton(this.keyMap.get(evt.key), isModifierless(evt));
            }
        });
        window.addEventListener("keyup", (evt) => {
            if (this.keyMap.has(evt.key)) {
                this.setButton(this.keyMap.get(evt.key), false);
            }
        });
        this.mayTeleport = true;
        Object.seal(this);
    }
    updatePointerOrientation() {
        if (this.isPointerLocked) {
            this.position
                .set(this.env.renderer.domElement.clientWidth, this.env.renderer.domElement.clientHeight)
                .multiplyScalar(0.5);
        }
        super.updatePointerOrientation();
    }
    onUpdate() {
        super.onUpdate();
        this.env.avatar.zoom(this.dz);
        this.dz = 0;
    }
    get isPointerLocked() {
        return document.pointerLockElement != null;
    }
    get isPointerCaptured() {
        return this.element.hasPointerCapture(this.pointerID);
    }
    get canDragView() {
        return super.canDragView && !this.isPointerLocked;
    }
    get canTeleport() {
        return super.canTeleport && this.isPointerLocked;
    }
    lockPointer() {
        this.element.requestPointerLock();
    }
    unlockPointer() {
        document.exitPointerLock();
    }
    capturePointer() {
        this.element.setPointerCapture(this.pointerID);
    }
    releaseCapture() {
        this.element.releasePointerCapture(this.pointerID);
    }
    vibrate() {
        // do nothing
    }
}
//# sourceMappingURL=PointerMouse.js.map