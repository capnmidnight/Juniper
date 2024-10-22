import { Vector2 } from "three";
import { resolveCamera } from "../../resolveCamera";
import { BasePointer } from "./BasePointer";
export class BaseScreenPointer extends BasePointer {
    constructor(type, id, env, cursor) {
        super(type, id, env, cursor);
        this.position = new Vector2();
        this.motion = new Vector2();
        this.uv = new Vector2();
        this.duv = new Vector2();
        this.uvComp = new Vector2(1, -1);
        this.uvOff = new Vector2(-1, 1);
        this.canMoveView = true;
        const onPointerEvent = (evt) => {
            this._isActive = this.onCheckEvent(evt);
            if (this._isActive) {
                this.onReadEvent(evt);
            }
        };
        this.element = this.env.renderer.domElement;
        this.element.addEventListener("pointerdown", onPointerEvent);
        this.element.addEventListener("pointermove", onPointerEvent);
        this.element.addEventListener("pointerup", onPointerEvent);
        this.element.addEventListener("pointercancel", onPointerEvent);
    }
    onCheckEvent(evt) {
        return evt.pointerType === this.type;
    }
    onReadEvent(_evt) {
        this.updatePointerOrientation();
    }
    updatePointerOrientation() {
        if (this.element.clientWidth > 0
            && this.element.clientHeight > 0) {
            this.uv.copy(this.position);
            this.uv.x /= this.element.clientWidth;
            this.uv.y /= this.element.clientHeight;
            this.uv
                .multiplyScalar(2)
                .multiply(this.uvComp)
                .add(this.uvOff);
            this.duv.copy(this.motion);
            this.duv.x /= this.element.clientWidth;
            this.duv.y /= this.element.clientHeight;
            this.duv
                .multiplyScalar(2)
                .multiply(this.uvComp);
            this.moveDistance = 200 * this.duv.length();
        }
        const cam = resolveCamera(this.env.renderer, this.env.camera);
        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction
            .set(this.uv.x, this.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
        this.up
            .set(0, 1, 0)
            .applyQuaternion(this.env.avatar.worldQuat);
    }
    onUpdate() {
        this.env.avatar.onMove(this, this.uv, this.duv);
        super.onUpdate();
        this.motion.setScalar(0);
        this.duv.setScalar(0);
    }
}
//# sourceMappingURL=BaseScreenPointer.js.map