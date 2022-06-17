import { PointerEventTypes } from "@juniper-lib/threejs/eventSystem/PointerEventTypes";
import { TypedEvent } from "@juniper-lib/tslib";
import { FlickEvent } from "./FlickEvent";
import type { IPointer } from "./IPointer";
import { ObjectMovedEvent } from "./ObjectMovedEvent";
import { getMeshTarget, RayTarget } from "./RayTarget";

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

    get hit(): THREE.Intersection {
        return this._hit;
    }

    set hit(v: THREE.Intersection) {
        if (v !== this.hit) {
            this._hit = v;
            this._point = null;
            this._distance = Number.POSITIVE_INFINITY;
            this._rayTarget = null;

            if (v) {
                this._point = v.point;
                this._distance = v.distance;
                this._rayTarget = getMeshTarget(v.object);
            }
        }
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
    dragstart: EventSystemEvent<"dragstart">;
    drag: EventSystemEvent<"drag">;
    dragcancel: EventSystemEvent<"dragcancel">;
    dragend: EventSystemEvent<"dragend">;
    objectMoved: ObjectMovedEvent;
    flick: FlickEvent;
}