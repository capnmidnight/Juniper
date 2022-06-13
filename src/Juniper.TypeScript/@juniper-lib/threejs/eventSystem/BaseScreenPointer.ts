import { PointerName } from "@juniper-lib/event-system/PointerName";
import { VirtualButtons } from "@juniper-lib/event-system/VirtualButtons";
import {
    project,
    unproject
} from "@juniper-lib/tslib";
import { resolveCamera } from "../resolveCamera";
import type { BaseCursor } from "./BaseCursor";
import { BasePointer } from "./BasePointer";
import type { EventSystem } from "./EventSystem";
import type { PointerType } from "./IPointer";

export abstract class BaseScreenPointer extends BasePointer {
    id: number = null;
    element: HTMLCanvasElement;

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

        const onPointerMove = (evt: PointerEvent) => {
            if (this.checkEvent(evt)) {
                this.readEvent(evt);
                this.onPointerMove();
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
        //this.element.addEventListener("pointercancel", onPointerUp);
    }

    get isTracking() {
        return this.id != null;
    }

    protected checkEvent(evt: PointerEvent) {
        return evt.pointerType === this.type
            && evt.pointerId === this.id;
    }

    protected readEvent(evt: PointerEvent) {
        if (this.checkEvent(evt)) {
            this.basicReadEvent(evt);

            this.state.buttons = evt.buttons;
            this.state.x = evt.offsetX;
            this.state.y = evt.offsetY;
            this.state.dx = evt.movementX;
            this.state.dy = evt.movementY;

            this.stateDelta(evt.type);
        }
    }

    protected basicReadEvent(evt: PointerEvent) {
        this.state.ctrl = evt.ctrlKey;
        this.state.alt = evt.altKey;
        this.state.shift = evt.shiftKey;
        this.state.meta = evt.metaKey;
        this.state.dz = 0;
    }

    protected stateDelta(type: string) {
        if (type === "pointermove" && this.lastState) {
            if (document.pointerLockElement) {
                this.state.x = this.lastState.x + this.state.dx;
                this.state.y = this.lastState.y + this.state.dy;
            }
            else {
                this.state.dx = this.state.x - this.lastState.x;
                this.state.dy = this.state.y - this.lastState.y;
            }
        }

        this.state.moveDistance = Math.sqrt(
            this.state.dx * this.state.dx
            + this.state.dy * this.state.dy);

        this.state.u = unproject(project(this.state.x, 0, this.element.clientWidth), -1, 1);
        this.state.v = unproject(project(this.state.y, 0, this.element.clientHeight), -1, 1);

        this.state.du = 2 * this.state.dx / this.element.clientWidth;
        this.state.dv = 2 * this.state.dy / this.element.clientHeight;
    }

    private moveOnUpdate = false;

    override recheck(): void {
        this.moveOnUpdate = true;
        super.recheck();
    }

    protected override onUpdate() {
        super.onUpdate();
        const cam = resolveCamera(this.renderer, this.camera);

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction.set(this.state.u, -this.state.v, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();

        if (this.moveOnUpdate) {
            this.onPointerMove();
        }

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction.set(this.state.u, -this.state.v, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
    }

    protected override onPointerMove() {
        this.moveOnUpdate = false;
        super.onPointerMove();
    }

    isPressed(button: VirtualButtons): boolean {
        const mask = 1 << button;
        return this.state.buttons === mask;
    }
}
