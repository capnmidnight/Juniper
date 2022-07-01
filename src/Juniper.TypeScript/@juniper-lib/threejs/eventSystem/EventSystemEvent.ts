import type { PointerEventTypes } from "@juniper-lib/threejs/eventSystem/PointerEventTypes";
import { TypedEvent } from "@juniper-lib/tslib";
import type { IPointer } from "./IPointer";
import type { RayTarget } from "./RayTarget";

export class EventSystemEvent<T extends PointerEventTypes = PointerEventTypes> extends TypedEvent<T> {
    private _hit: THREE.Intersection = null;
    private _point: THREE.Vector3 = null;
    private _distance: number = Number.POSITIVE_INFINITY;
    private _rayTarget: RayTarget = null;

    constructor(type: T,
        public readonly pointer: IPointer) {
        super(type);

        Object.seal(this);
    }

    set(v: THREE.Intersection, t: RayTarget) {
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

    get hit(): THREE.Intersection {
        return this._hit;
    }

    get rayTarget(): RayTarget {
        return this._rayTarget;
    }

    get point(): THREE.Vector3 {
        return this._point;
    }

    get distance(): number {
        return this._distance;
    }
}

export interface EventSystemEvents {
    move: EventSystemEvent<"move">;
    enter: EventSystemEvent<"enter">;
    exit: EventSystemEvent<"exit">;
    up: EventSystemEvent<"up">;
    down: EventSystemEvent<"down">;
    click: EventSystemEvent<"click">;
}