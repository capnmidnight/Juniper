import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointer } from "./BaseScreenPointer";
import { CursorXRMouse } from "./CursorXRMouse";
import { PointerType } from "./IPointer";

export abstract class BaseScreenPointerSinglePoint extends BaseScreenPointer {
    constructor(type: PointerType, name: PointerName, env: BaseEnvironment) {

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

        const element = env.renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);

        super(type, name, env, new CursorXRMouse(env.renderer));

        element.addEventListener("pointerup", unPrep);
        element.addEventListener("pointercancel", unPrep);

        this.canMoveView = true;
    }

    protected onReadEvent(evt: PointerEvent) {
        this._buttons = evt.buttons;
        this.position.set(evt.offsetX, evt.offsetY);
        this.motion.set(evt.movementX, evt.movementY);
    }
}
