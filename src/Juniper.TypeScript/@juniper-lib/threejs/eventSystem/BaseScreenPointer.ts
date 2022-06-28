import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { resolveCamera } from "../resolveCamera";
import type { BaseCursor } from "./BaseCursor";
import { BasePointer } from "./BasePointer";
import type { PointerType } from "./IPointer";

export abstract class BaseScreenPointer extends BasePointer {
    id: number = null;
    element: HTMLCanvasElement;

    protected readonly position = new THREE.Vector2();
    protected readonly motion = new THREE.Vector2();
    private readonly uv = new THREE.Vector2();
    private readonly duv = new THREE.Vector2();
    private readonly uvComp = new THREE.Vector2(1, -1);
    private readonly uvOff = new THREE.Vector2(-1, 1);
    private lastUV: THREE.Vector2 = null;

    constructor(
        type: PointerType,
        name: PointerName,
        env: BaseEnvironment,
        cursor: BaseCursor) {
        super(type, name, env, cursor);

        const onPointerDown = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
                this.onPointerDown();
            }
        };

        this.element = this.env.renderer.domElement;
        this.element.addEventListener("pointerdown", onPointerDown);

        const onPointerMove = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
            }
        };

        this.element.addEventListener("pointermove", onPointerMove);

        const onPointerUp = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
                this.onPointerUp();
            }
        };

        this.element.addEventListener("pointerup", onPointerUp);
        this.element.addEventListener("pointercancel", onPointerUp);
    }

    private checkEvent(evt: PointerEvent): boolean {
        return this.isActive = this.onCheckEvent(evt);
    }

    protected onCheckEvent(evt: PointerEvent): boolean {
        return evt.pointerType === this.type
            && evt.pointerId === this.id;
    }

    get isTracking() {
        return this.id != null;
    }

    isPressed(button: VirtualButtons): boolean {
        const mask = 1 << button;
        return this.buttons === mask;
    }

    private readEvent(evt: PointerEvent): void {
        if (this.checkEvent(evt)) {
            this.onReadEvent(evt);

            if (this.element.clientWidth > 0
                && this.element.clientHeight > 0) {
                this.uv
                    .copy(this.position)
                    .multiplyScalar(2);

                this.uv.x /= this.element.clientWidth;
                this.uv.y /= this.element.clientHeight;

                this.uv
                    .multiply(this.uvComp)
                    .add(this.uvOff);
            }
        }
    }

    protected override onPointerMove() {
        super.onPointerMove();

        if (this.lastUV) {
            this.duv.copy(this.uv)
                .sub(this.lastUV);

            this.lastUV.copy(this.uv);
        }

        this.env.avatar.onMove(this, this.uv, this.duv);

        if (!this.lastUV) {
            this.lastUV = new THREE.Vector2()
                .copy(this.uv);
        }
    }

    protected abstract onReadEvent(evt: PointerEvent): void;

    protected onUpdate(): void {
        const cam = resolveCamera(this.env.renderer, this.env.camera);

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction
            .set(this.uv.x, this.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
    }
}
