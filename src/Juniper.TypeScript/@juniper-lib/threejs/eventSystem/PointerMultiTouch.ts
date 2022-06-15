import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointer } from "./BaseScreenPointer";
import type { EventSystem } from "./EventSystem";

function dist(a: PointerEvent, b: PointerEvent) {
    const dx = b.offsetX - a.offsetY;
    const dy = b.offsetY - a.offsetY;
    return Math.sqrt(dx * dx + dy * dy);
}

export class PointerMultiTouch extends BaseScreenPointer {
    private lastPinchDist = 0;
    private readonly points = new Map<number, PointerEvent>();

    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {
        super("touch", PointerName.Touches, evtSys, renderer, camera, null);
        this.canMoveView = true;
        Object.seal(this);
    }

    protected override checkEvent(evt: PointerEvent) {
        return evt.pointerType === this.type;
    }

    protected override onReadEvent(evt: PointerEvent) {
        if (evt.type === "pointerdown" || evt.type === "pointermove") {
            this.points.set(evt.pointerId, evt);
        }
        else if (this.points.has(evt.pointerId)
            && (evt.type === "pointerup" || evt.type === "pointercancel")) {
            this.points.delete(evt.pointerId);
        }

        this.state.buttons = 0;

        if (this.points.size > 0) {
            this.state.position.setScalar(0);

            const K = 1 / this.points.size;
            for (const point of this.points.values()) {
                this.state.buttons |= point.buttons << (this.points.size - 1);
                this.state.position.x += K * point.offsetX;
                this.state.position.y -= K * point.offsetY;
                this.state.motion.x += K * point.movementX;
                this.state.motion.y -= K * point.movementY;
            }

            if (this.points.size === 2) {
                const [a, b] = Array.from(this.points.values());
                const pinchDist = dist(a, b);
                if (this.lastState && this.lastState.buttons === 2) {
                    this.state.dz = (pinchDist - this.lastPinchDist) * 2.5;
                }

                this.lastPinchDist = pinchDist;
            }
        }
    }

    override vibrate() {
        navigator.vibrate(125);
    }
}
