import { TypedEvent } from "@juniper/tslib";
import type { IPointer } from "./IPointer";
import { resolveObj } from "./resolveObj";

export class EventSystemEvent<T extends string> extends TypedEvent<T> {
    private _hit: THREE.Intersection = null;
    private _point: THREE.Vector3 = null;
    private _distance: number = Number.POSITIVE_INFINITY;
    private _object: THREE.Object3D<THREE.Event> = null;

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
            this._object = null;

            if (v) {
                this._point = v.point;
                this._distance = v.distance;
                this._object = resolveObj(v);
            }
        }
    }

    get object(): THREE.Object3D {
        return this._object;
    }

    get point(): THREE.Vector3 {
        return this._point;
    }

    get distance(): number {
        return this._distance;
    }

    to3(altHit: THREE.Intersection): EventSystemThreeJSEvent<T> {
        return new EventSystemThreeJSEvent(this.type, altHit, this.pointer.state.buttons);
    }
}

export class EventSystemThreeJSEvent<T extends string> implements THREE.Event {
    private _point: THREE.Vector3;
    private _distance: number;
    private _object: THREE.Object3D;

    constructor(public readonly type: T,
        public readonly hit: THREE.Intersection,
        public readonly buttons: number) {
        this._point = this.hit && this.hit.point;
        this._distance = this.hit && this.hit.distance || Number.POSITIVE_INFINITY;
        this._object = resolveObj(this.hit);
    }

    get object(): THREE.Object3D {
        return this._object;
    }

    get point(): THREE.Vector3 {
        return this._point;
    }

    get distance(): number {
        return this._distance;
    }
}