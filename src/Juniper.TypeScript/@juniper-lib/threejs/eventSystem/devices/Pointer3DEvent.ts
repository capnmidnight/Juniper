import { TypedEvent } from "@juniper-lib/events/TypedEventBase";
import { Intersection, Vector3 } from "three";
import type { RayTarget } from "../RayTarget";
import type { IPointer } from "./IPointer";
import type { PointerEventTypes } from "./PointerEventTypes";

export class Pointer3DEvent<T extends PointerEventTypes = PointerEventTypes> extends TypedEvent<T> {
    private _hit: Intersection = null;
    private _point: Vector3 = null;
    private _distance: number = Number.POSITIVE_INFINITY;
    private _rayTarget: RayTarget = null;

    constructor(type: T,
        public readonly pointer: IPointer) {
        super(type);

        Object.seal(this);
    }

    set(v: Intersection, t: RayTarget) {
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

    get hit(): Intersection {
        return this._hit;
    }

    get rayTarget(): RayTarget {
        return this._rayTarget;
    }

    get point(): Vector3 {
        return this._point;
    }

    get distance(): number {
        return this._distance;
    }
}

export type Pointer3DEvents = {
    move: Pointer3DEvent<"move">;
    enter: Pointer3DEvent<"enter">;
    exit: Pointer3DEvent<"exit">;
    up: Pointer3DEvent<"up">;
    down: Pointer3DEvent<"down">;
    click: Pointer3DEvent<"click">;
}