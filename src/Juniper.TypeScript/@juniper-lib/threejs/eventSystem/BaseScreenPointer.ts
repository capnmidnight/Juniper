import { PointerID, PointerType } from "@juniper-lib/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { resolveCamera } from "../resolveCamera";
import type { BaseCursor } from "./BaseCursor";
import { BasePointer } from "./BasePointer";

export abstract class BaseScreenPointer extends BasePointer {
    element: HTMLCanvasElement;

    protected readonly position = new THREE.Vector2();
    protected readonly motion = new THREE.Vector2();
    private readonly uv = new THREE.Vector2();
    private readonly duv = new THREE.Vector2();
    private readonly uvComp = new THREE.Vector2(1, -1);
    private readonly uvOff = new THREE.Vector2(-1, 1);

    constructor(
        type: PointerType,
        id: PointerID,
        env: BaseEnvironment,
        cursor: BaseCursor) {
        super(type, id, env, cursor);

        this.canMoveView = true;

        const onPointerEvent = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
            }
        }

        this.element = this.env.renderer.domElement;
        this.element.addEventListener("pointerdown", onPointerEvent);
        this.element.addEventListener("pointermove", onPointerEvent);
        this.element.addEventListener("pointerup", onPointerEvent);
        this.element.addEventListener("pointercancel", onPointerEvent);
    }

    private checkEvent(evt: PointerEvent): boolean {
        return this.isActive = this.onCheckEvent(evt);
    }

    protected onCheckEvent(evt: PointerEvent): boolean {
        return evt.pointerType === this.type;
    }

    private readEvent(evt: PointerEvent): void {
        if (this.checkEvent(evt)) {
            this.onReadEvent(evt);
        }
    }

    protected onReadEvent(_evt: PointerEvent): void {
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

        this.updatePointerOrientation();
    }

    protected updatePointerOrientation() {
        const cam = resolveCamera(this.env.renderer, this.env.camera);
        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction
            .set(this.uv.x, this.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();

        this.up
            .set(0, 1, 0)
            .applyQuaternion(this.env.avatar.worldQuat)
    }

    protected override onUpdate(): void {
        this.env.avatar.onMove(this, this.uv, this.duv);

        super.onUpdate();

        this.motion.setScalar(0);
        this.duv.setScalar(0);
    }
}
