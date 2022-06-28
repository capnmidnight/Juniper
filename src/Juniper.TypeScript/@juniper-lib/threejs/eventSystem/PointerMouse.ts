import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";

export class PointerMouse extends BaseScreenPointerSinglePoint {
    allowPointerLock: boolean = false;

    private lastPosition: THREE.Vector2 = null;

    constructor(env: BaseEnvironment) {
        super("mouse", PointerName.Mouse, env);

        this.element.addEventListener("wheel", (evt: WheelEvent) => {
            evt.preventDefault();
            this.env.avatar.zoom(-evt.deltaY * 0.5);
        }, { passive: false });

        this.element.addEventListener("contextmenu", (evt: Event) => {
            evt.preventDefault();
        });

        this.element.addEventListener("pointerdown", (evt) => {
            if (this.onCheckEvent(evt)
                && this.allowPointerLock
                && !this.isPointerLocked) {
                this.lockPointer();
            }
        });

        document.addEventListener("pointerlockchange", () => {
            if (this.isPointerLocked) {
                this.lastPosition = new THREE.Vector2()
                    .copy(this.position);
            }
            else {
                this.lastPosition = null;
            }
        })

        Object.seal(this);
    }

    protected override onReadEvent(evt: PointerEvent): void {
        super.onReadEvent(evt);

        if (this.lastPosition) {
            this.position
                .copy(this.lastPosition)
                .add(this.motion);

            this.lastPosition.copy(this.position);
        }
    }

    get isPointerLocked() {
        return document.pointerLockElement != null;
    }

    lockPointer() {
        this.element.requestPointerLock();
    }

    unlockPointer() {
        document.exitPointerLock();
    }
}
