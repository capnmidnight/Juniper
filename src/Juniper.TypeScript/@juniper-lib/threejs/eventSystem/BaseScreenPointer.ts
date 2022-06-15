import { VirtualButtons } from "@juniper-lib/threejs/eventSystem/VirtualButtons";
import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { resolveCamera } from "../resolveCamera";
import type { BaseCursor } from "./BaseCursor";
import { BasePointer } from "./BasePointer";
import type { EventSystem } from "./EventSystem";
import type { PointerType } from "./IPointer";

export abstract class BaseScreenPointer extends BasePointer {
    id: number = null;
    element: HTMLCanvasElement;

    private readonly sizeInv = new THREE.Vector2();

    constructor(
        type: PointerType,
        name: PointerName,
        evtSys: EventSystem,
        protected readonly renderer: THREE.WebGLRenderer,
        protected readonly camera: THREE.PerspectiveCamera,
        cursor: BaseCursor) {
        super(type, name, evtSys, cursor);

        const onPointerDown = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
                this.onPointerDown();
            }
        };

        this.element = this.renderer.domElement;
        this.element.addEventListener("pointerdown", onPointerDown);

        const setSizeInv = () => this.sizeInv.set(1 / this.element.clientWidth, 1 / this.element.clientHeight);
        this.element.addEventListener("resize", setSizeInv);
        setSizeInv();        

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

    get isTracking() {
        return this.id != null;
    }

    isPressed(button: VirtualButtons): boolean {
        const mask = 1 << button;
        return this.state.buttons === mask;
    }

    private readEvent(evt: PointerEvent): void {
        if(this.checkEvent(evt)) {
            this.readMetaKeys(evt);
            this.onReadEvent(evt);
            this.stateDelta(evt.type);
        }
    }

    protected checkEvent(evt: PointerEvent) {
        return evt.pointerType === this.type
            && evt.pointerId === this.id;
    }

    protected readMetaKeys(evt: PointerEvent) {
        this.state.ctrl = evt.ctrlKey;
        this.state.alt = evt.altKey;
        this.state.shift = evt.shiftKey;
        this.state.meta = evt.metaKey;
    }

    protected abstract onReadEvent(evt: PointerEvent): void;

    protected stateDelta(type: string) {
        if (type === "pointermove"
            && document.pointerLockElement
            && this.lastState) {
            this.state.position
                .copy(this.lastState.position)
                .add(this.state.motion);
        }

        this.state.moveDistance = this.state.motion.length();

        this.state.uv
            .copy(this.state.position)
            .multiplyScalar(2)
            .multiply(this.sizeInv)
            .addScalar(-1);

        this.state.duv
            .copy(this.state.motion)
            .multiplyScalar(2)
            .multiply(this.sizeInv);
    }

    protected onUpdate() {
        const cam = resolveCamera(this.renderer, this.camera);

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction.set(this.state.uv.x, -this.state.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();

        if (this.state.motion.manhattanLength() > 0) {
            this.onPointerMove();
        }

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction.set(this.state.uv.x, -this.state.uv.y, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
    }
}
