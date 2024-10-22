import { Vector2 } from "three";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { resolveCamera } from "../../resolveCamera";
import type { BaseCursor3D } from "../cursors/BaseCursor3D";
import { PointerID, PointerType } from "../Pointers";
import { BasePointer } from "./BasePointer";

export abstract class BaseScreenPointer extends BasePointer {
    element: HTMLCanvasElement;

    protected readonly position = new Vector2();
    protected readonly motion = new Vector2();
    private readonly uv = new Vector2();
    private readonly duv = new Vector2();
    private readonly uvComp = new Vector2(1, -1);
    private readonly uvOff = new Vector2(-1, 1);

    constructor(
        type: PointerType,
        id: PointerID,
        env: BaseEnvironment,
        cursor: BaseCursor3D) {
        super(type, id, env, cursor);

        this.canMoveView = true;

        const onPointerEvent = (evt: PointerEvent) => {
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

    protected onCheckEvent(evt: PointerEvent): boolean {
        return evt.pointerType === this.type;
    }

    protected onReadEvent(_evt: PointerEvent): void {
        this.updatePointerOrientation();
    }

    protected updatePointerOrientation() {
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

    protected override onUpdate(): void {
        this.env.avatar.onMove(this, this.uv, this.duv);

        super.onUpdate();

        this.motion.setScalar(0);
        this.duv.setScalar(0);
    }
}
