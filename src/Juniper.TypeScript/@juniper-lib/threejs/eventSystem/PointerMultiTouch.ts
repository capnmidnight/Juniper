import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { BaseScreenPointer } from "./BaseScreenPointer";

function dist(a: PointerEvent, b: PointerEvent) {
    const dx = b.offsetX - a.offsetY;
    const dy = b.offsetY - a.offsetY;
    return Math.sqrt(dx * dx + dy * dy);
}

export class PointerMultiTouch extends BaseScreenPointer {
    private lastPinchDist = 0;
    private readonly points = new Map<number, PointerEvent>();

    constructor(env: BaseEnvironment) {
        super("touch", PointerName.Touches, env, null);
        this.canMoveView = true;
        Object.seal(this);
    }

    protected override onCheckEvent(evt: PointerEvent) {
        return evt.pointerType === this.type;
    }

    protected onReadEvent(evt: PointerEvent) {
        if (evt.type === "pointerdown" || evt.type === "pointermove") {
            this.points.set(evt.pointerId, evt);
        }
        else if (this.points.has(evt.pointerId)
            && (evt.type === "pointerup" || evt.type === "pointercancel")) {
            this.points.delete(evt.pointerId);
        }

        this._buttons = 0;

        if (this.points.size > 0) {
            this.position.setScalar(0);

            const K = 1 / this.points.size;
            for (const point of this.points.values()) {
                this._buttons |= point.buttons << (this.points.size - 1);
                this.position.x += K * point.offsetX;
                this.position.y += K * point.offsetY;
                this.motion.x += K * point.movementX;
                this.motion.y += K * point.movementY;
            }

            if (this.points.size === 2) {
                const [a, b] = Array.from(this.points.values());
                const pinchDist = dist(a, b);
                if (evt.type === "pointermove") {
                    this.env.fovControl.zoom((pinchDist - this.lastPinchDist) * 5);
                }

                this.lastPinchDist = pinchDist;
            }
        }
    }

    override vibrate() {
        navigator.vibrate(125);
    }
}
