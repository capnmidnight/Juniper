import { arrayClear, arrayRemoveByKey } from "@juniper-lib/tslib/collections/arrays";
import { PointerID } from "@juniper-lib/tslib/events/Pointers";
import { isFunction } from "@juniper-lib/tslib/typeChecks";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseScreenPointer } from "./BaseScreenPointer";


function getPointerID(evt: PointerEvent) {
    return evt.pointerId;
}

export class PointerTouch
    extends BaseScreenPointer {

    private dz = 0;
    private lastZ = 0;
    private readonly points = new Array<PointerEvent>();

    constructor(env: BaseEnvironment) {
        super("touch", PointerID.Touch, env, null);

        Object.seal(this);
    }

    override get enabled() {
        return super.enabled;
    }

    override set enabled(v) {
        super.enabled = v;
        arrayClear(this.points);
    }

    private readonly lastXs = new Map<number, number>();
    private readonly lastYs = new Map<number, number>();

    protected override onReadEvent(evt: PointerEvent): void {
        arrayRemoveByKey(this.points, evt.pointerId, getPointerID);
        const isMove = evt.type === "pointerdown" || evt.type === "pointermove";
        if (isMove) {
            this.points.push(evt);
        }

        if (this.points.length === 2) {
            const a = this.points[0];
            const b = this.points[1];
            const dx = b.offsetX - a.offsetX;
            const dy = b.offsetY - a.offsetY;
            const z = 5 * Math.sqrt(dx * dx + dy * dy);
            const ddz = z - this.lastZ;
            if (evt.type === "pointermove") {
                this.dz += ddz;
            }
            this.lastZ = z;
        }

        const K = 1 / this.points.length;

        if (isMove) {
            this.position.setScalar(0);

            for (const point of this.points) {
                this.position.x += K * point.offsetX;
                this.position.y += K * point.offsetY;
                if (this.lastXs.has(point.pointerId)) {
                    const lastX = this.lastXs.get(point.pointerId);
                    const lastY = this.lastYs.get(point.pointerId);
                    const dx = point.offsetX - lastX;
                    const dy = point.offsetY - lastY;
                    this.motion.x += K * dx;
                    this.motion.y += K * dy;
                }
            }
        }

        if (isMove) {
            this.lastXs.set(evt.pointerId, evt.offsetX);
            this.lastYs.set(evt.pointerId, evt.offsetY);
        }
        else {
            this.lastXs.delete(evt.pointerId);
            this.lastYs.delete(evt.pointerId);
        }

        super.onReadEvent(evt);

        if (evt.type !== "pointermove") {
            let curButtons = 0;
            for (let button = 0; button < this.points.length; ++button) {
                const point = this.points[button];
                const mask = 1 << button;
                if (point.buttons !== 0) {
                    curButtons |= mask;
                }
                else {
                    curButtons &= ~mask;
                }

            }

            for (let button = 0; button < 10; ++button) {
                const wasPressed = this.isPressed(button);
                const mask = 1 << button;
                const isPressed = (curButtons & mask) !== 0;
                if (isPressed !== wasPressed) {
                    this.setButton(button, isPressed);
                }
            }
        }
    }

    protected override onUpdate(): void {
        this.env.avatar.zoom(this.dz);

        super.onUpdate();

        this.dz = 0;
    }

    private _canVibrate: boolean = null;
    private get canVibrate() {
        if (this._canVibrate === null) {
            this._canVibrate = "vibrate" in navigator
                && isFunction(navigator.vibrate);
        }
        return this._canVibrate;
    }

    vibrate() {
        if (this.canVibrate) {
            navigator.vibrate(125);
        }
    }
}
