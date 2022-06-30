import { isModifierless } from "@juniper-lib/dom/evts";
import { PointerID } from "@juniper-lib/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
import { VirtualButton } from "./VirtualButton";

export class PointerMouse extends BaseScreenPointerSinglePoint {
    allowPointerLock: boolean = false;

    private dz = 0;
    private lastPosition: THREE.Vector2 = null;

    private readonly keyMap = new Map<string, VirtualButton>([
        ["`", VirtualButton.Info],
        ["ContextMenu", VirtualButton.Menu]
    ]);

    constructor(env: BaseEnvironment) {
        super("mouse", PointerID.Mouse, env);

        this.element.addEventListener("wheel", (evt: WheelEvent) => {
            evt.preventDefault();
            this.dz += -evt.deltaY * 0.1;
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
                this.lastPosition = this.position.clone();
            }
            else {
                this.lastPosition = null;
            }
        });

        window.addEventListener("keydown", (evt) => {
            if (this.isActive && this.keyMap.has(evt.key)) {
                this.setButton(this.keyMap.get(evt.key), isModifierless(evt));
            }
        });

        window.addEventListener("keyup", (evt) => {
            if (this.keyMap.has(evt.key)) {
                this.setButton(this.keyMap.get(evt.key), false);
            }
        });

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

    protected override onUpdate() {
        super.onUpdate();
        this.env.avatar.zoom(this.dz);
        this.dz = 0;
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

    vibrate() {
        // do nothing
    }
}
