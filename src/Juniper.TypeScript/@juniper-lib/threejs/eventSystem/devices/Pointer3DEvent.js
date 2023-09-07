import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export class Pointer3DEvent extends TypedEvent {
    constructor(type, pointer) {
        super(type);
        this.pointer = pointer;
        this._hit = null;
        this._point = null;
        this._distance = Number.POSITIVE_INFINITY;
        this._rayTarget = null;
        Object.seal(this);
    }
    set(v, t) {
        if (v !== this.hit) {
            this._hit = v;
            if (v) {
                this._point = v.point;
                this._distance = v.distance;
            }
            else {
                this._point = null;
                this._distance = Number.POSITIVE_INFINITY;
            }
        }
        this._rayTarget = t;
    }
    get hit() {
        return this._hit;
    }
    get rayTarget() {
        return this._rayTarget;
    }
    get point() {
        return this._point;
    }
    get distance() {
        return this._distance;
    }
}
//# sourceMappingURL=Pointer3DEvent.js.map