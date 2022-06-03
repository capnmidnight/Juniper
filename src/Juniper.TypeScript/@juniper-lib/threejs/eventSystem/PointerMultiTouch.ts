import { PointerName } from "@juniper-lib/event-system/PointerName";
import { PointerState } from "@juniper-lib/event-system/PointerState";
import { VirtualButtons } from "@juniper-lib/event-system/VirtualButtons";
import { resolveCamera } from "../resolveCamera";
import { BasePointer } from "./BasePointer";
import type { EventSystem } from "./EventSystem";
import { countPresses, PointerSingleTouch } from "./PointerSingleTouch";

function dist(a: PointerState, b: PointerState) {
    const dx = b.x - a.x;
    const dy = b.y - a.y;
    return Math.sqrt(dx * dx + dy * dy);
}

export class PointerMultiTouch extends BasePointer {
    private touches = new Array<PointerSingleTouch>(10);
    private lastPinchDist = 0;
    private lastPressCount = 0;

    constructor(evtSys: EventSystem, private readonly renderer: THREE.WebGLRenderer, private readonly camera: THREE.PerspectiveCamera) {
        super("touch", PointerName.Touches, evtSys, null);

        for (let i = 0; i < this.touches.length; ++i) {
            this.touches[i] = new PointerSingleTouch(evtSys, renderer, i, camera, this);
        }

        this.canMoveView = true;

        Object.seal(this);
    }

    onDown() {
        this.updateState();
        this.onPointerDown();
    }

    onUp() {
        this.updateState();
        this.onPointerUp();
    }

    onMove() {
        this.updateState();
        this.onPointerMove();

        const pressCount = countPresses();
        if (pressCount === 2) {
            let a: PointerSingleTouch = null;
            let b: PointerSingleTouch = null;
            for (const touch of this.touches) {
                if (touch.isTracking) {
                    if (!a) {
                        a = touch;
                    }
                    else if (!b) {
                        b = touch;
                        break;
                    }
                }
            }

            const pinchDist = dist(a.state, b.state);
            const dz = (pinchDist - this.lastPinchDist) * 2.5;
            if (this.lastPressCount === 2) {
                this.onZoom(dz);
            }

            this.lastPinchDist = pinchDist;
        }

        this.lastPressCount = pressCount;
    }

    override vibrate() {
        navigator.vibrate(125);
    }

    private updateState(): void {
        this.lastStateUpdate(() => {
            this.state.buttons = 0;

            this.state.alt = false;
            this.state.ctrl = false;
            this.state.shift = false;
            this.state.meta = false;
            this.state.canClick = false;
            this.state.dragging = false;

            this.state.dragDistance = 0;
            this.state.moveDistance = 0;
            this.state.x = 0;
            this.state.y = 0;
            this.state.dx = 0;
            this.state.dy = 0;
            this.state.u = 0;
            this.state.v = 0;
            this.state.du = 0;
            this.state.dv = 0;

            const K = 1 / countPresses();
            for (const touch of this.touches) {
                if (touch.isTracking) {
                    this.state.buttons = Math.max(this.state.buttons, touch.state.buttons);

                    this.state.alt = this.state.alt || touch.state.alt;
                    this.state.ctrl = this.state.ctrl || touch.state.ctrl;
                    this.state.shift = this.state.shift || touch.state.shift;
                    this.state.meta = this.state.meta || touch.state.meta;
                    this.state.canClick = this.state.canClick || touch.state.canClick;
                    this.state.dragging = this.state.dragging || touch.state.dragging;

                    this.state.dragDistance += K * touch.state.dragDistance;
                    this.state.moveDistance += K * touch.state.moveDistance;
                    this.state.x += K * touch.state.x;
                    this.state.y += K * touch.state.y;
                    this.state.dx += K * touch.state.dx;
                    this.state.dy += K * touch.state.dy;
                    this.state.u += K * touch.state.u;
                    this.state.v += K * touch.state.v;
                    this.state.du += K * touch.state.du;
                    this.state.dv += K * touch.state.dv;
                }
            }
        });
    }

    override get canMoveView() {
        return super.canMoveView;
    }

    override set canMoveView(v) {
        if (this.touches) {
            for (const touch of this.touches) {
                touch.canMoveView = v;
            }
            super.canMoveView = v;
        }
    }

    override get enabled() {
        return super.enabled;
    }

    override set enabled(v) {
        if (this.touches) {
            for (const touch of this.touches) {
                touch.enabled = v;
            }
        }
        super.enabled = v;
    }

    update() {
        const cam = resolveCamera(this.renderer, this.camera);

        this.origin.setFromMatrixPosition(cam.matrixWorld);
        this.direction.set(this.state.u, -this.state.v, 0.5)
            .unproject(cam)
            .sub(this.origin)
            .normalize();
    }

    isPressed(button: VirtualButtons) {
        for (const touch of this.touches) {
            if (touch.isPressed(button)) {
                return true;
            }
        }

        return false;
    }
}
