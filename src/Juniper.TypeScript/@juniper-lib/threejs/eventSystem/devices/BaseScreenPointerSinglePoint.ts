import { PointerID, PointerType } from "@juniper-lib/tslib/events/Pointers";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { CursorXRMouse } from "../cursors/CursorXRMouse";
import { BaseScreenPointer } from "./BaseScreenPointer";

export abstract class BaseScreenPointerSinglePoint extends BaseScreenPointer {

    protected pointerID: number = null;

    constructor(type: PointerType, id: PointerID, env: BaseEnvironment) {

        const onPrep = (evt: PointerEvent) => {
            if (evt.pointerType === type
                && this.pointerID == null) {
                this.pointerID = evt.pointerId;
            }
        };

        const unPrep = (evt: PointerEvent) => {
            if (evt.pointerType === type
                && this.pointerID != null) {
                this.pointerID = null;
            }
        };

        const element = env.renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);

        super(type, id, env, new CursorXRMouse(env));

        element.addEventListener("pointerup", unPrep);
        element.addEventListener("pointercancel", unPrep);
    }

    protected override onCheckEvent(evt: PointerEvent) {
        return super.onCheckEvent(evt)
            && evt.pointerId === this.pointerID;
    }

    protected override onReadEvent(evt: PointerEvent) {
        this.position.set(evt.offsetX, evt.offsetY);
        this.motion.x += evt.movementX;
        this.motion.y += evt.movementY;

        super.onReadEvent(evt);

        if (evt.type === "pointerdown"
            || evt.type === "pointerup"
            || evt.type === "pointercancel") {
            this.setButton(evt.button, evt.type === "pointerdown");
        }
    }
}