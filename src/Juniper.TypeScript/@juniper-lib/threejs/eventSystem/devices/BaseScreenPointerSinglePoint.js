import { CursorXRMouse } from "../cursors/CursorXRMouse";
import { BaseScreenPointer } from "./BaseScreenPointer";
export class BaseScreenPointerSinglePoint extends BaseScreenPointer {
    constructor(type, id, env) {
        const onPrep = (evt) => {
            if (evt.pointerType === type
                && this.pointerID == null) {
                this.pointerID = evt.pointerId;
            }
        };
        const unPrep = (evt) => {
            if (evt.pointerType === type
                && this.pointerID != null) {
                this.pointerID = null;
            }
        };
        const element = env.renderer.domElement;
        element.addEventListener("pointerdown", onPrep);
        element.addEventListener("pointermove", onPrep);
        super(type, id, env, new CursorXRMouse(env));
        this.pointerID = null;
        this.lastX = null;
        this.lastY = null;
        element.addEventListener("pointerup", unPrep);
        element.addEventListener("pointercancel", unPrep);
    }
    onCheckEvent(evt) {
        return super.onCheckEvent(evt)
            && evt.pointerId === this.pointerID;
    }
    onReadEvent(evt) {
        this.position.set(evt.offsetX, evt.offsetY);
        if (evt.type === "pointerdown") {
            this.motion.setScalar(0);
        }
        else {
            this.motion.x += evt.offsetX - this.lastX;
            this.motion.y += evt.offsetY - this.lastY;
        }
        this.lastX = evt.offsetX;
        this.lastY = evt.offsetY;
        super.onReadEvent(evt);
        if (evt.type === "pointerdown"
            || evt.type === "pointerup"
            || evt.type === "pointercancel") {
            this.setButton(evt.button, evt.type === "pointerdown");
        }
    }
}
//# sourceMappingURL=BaseScreenPointerSinglePoint.js.map