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
    private readonly sizeInv = new THREE.Vector2();
    private readonly uvComp = new THREE.Vector2(1, -1);
    private readonly uvOff = new THREE.Vector2(-1, 1);
    private lastPosition: THREE.Vector2 = null;

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

            if (evt.type === "pointermove"
                && document.pointerLockElement
                && this.lastPosition) {
                this.position
                    .copy(this.lastPosition)
                    .add(this.motion);
            }

            this.moveDistance = this.motion.length();

            this.sizeInv.set(this.element.clientWidth, this.element.clientHeight);
            if (this.sizeInv.manhattanLength() > 0) {
                this.uv
                    .copy(this.position)
                    .multiplyScalar(2)
                    .divide(this.sizeInv)
                    .multiply(this.uvComp)
                    .add(this.uvOff);
                this.duv
                    .copy(this.motion)
                    .multiplyScalar(2)
                    .divide(this.sizeInv)
                    .multiply(this.uvComp);
            }
        }
    }

    protected abstract onReadEvent(evt: PointerEvent): void;

    protected override onPointerMove(): void {
        this.env.avatar.onMove(this, this.uv, this.duv);
        super.onPointerMove();
    }

    protected onUpdate(): void {
        const cam = resolveCamera(this.env.renderer, this.env.camera);

        this.updateRay(cam);

        if (this.motion.manhattanLength() > 0) {
            this.onPointerMove();
            this.updateRay(cam);
        }

        if (!this.lastPosition) {
            this.lastPosition = new THREE.Vector2();
        }

        this.lastPosition.copy(this.position);
        this.motion.setScalar(0);
        this.duv.setScalar(0);
    }

    private updateRay(cam: THREE.Camera) {
        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction
            .set(this.uv.x, this.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
    }
}
