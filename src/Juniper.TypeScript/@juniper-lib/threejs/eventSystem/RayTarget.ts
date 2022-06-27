import { isDefined, isNumber, TypedEventBase } from "@juniper-lib/tslib";
import { ErsatzObject, isErsatzObject, objectResolve, Objects } from "../objects";
import { EventSystemEvents } from "./EventSystemEvent";

const RAY_TARGET_KEY = "Juniper:ThreeJS:EventSystem:RayTarget";

export class RayTarget<EventsT = void>
    extends TypedEventBase<EventsT & EventSystemEvents>
    implements ErsatzObject {

    private readonly meshes = new Array<THREE.Mesh>();

    private _disabled: boolean = false;
    private _clickable: boolean = false;
    private _draggable: boolean = false;

    constructor(public readonly object: THREE.Object3D) {
        super();
        this.object.userData[RAY_TARGET_KEY] = this;
    }

    addMesh(mesh: THREE.Mesh): this {
        mesh.userData[RAY_TARGET_KEY] = this;
        this.meshes.push(mesh);
        return this;
    }

    get disabled() {
        return this._disabled;
    }

    set disabled(v) {
        this._disabled = v;
    }

    get enabled() {
        return !this.disabled;
    }

    set enabled(v) {
        this.disabled = !v;
    }

    get clickable() {
        return this._clickable;
    }

    set clickable(v) {
        this._clickable = v;
    }

    get draggable() {
        return this._draggable;
    }

    set draggable(v) {
        this._draggable = v;
    }
}

export function isRayTarget<T = void>(obj: Objects): obj is RayTarget<T> {
    return obj instanceof RayTarget;
}

export function isIntersection(obj: any): obj is THREE.Intersection {
    return isDefined(obj)
        && isNumber(obj.distance)
        && obj.point instanceof THREE.Vector3
        && (obj.object === null
            || obj.object instanceof THREE.Object3D);
}

export function getRayTarget<T = void>(obj: Objects | THREE.Intersection): RayTarget<T> {
    if (!obj) {
        return null;
    }

    if (isRayTarget<T>(obj)) {
        if (obj.object.visible) {
            return obj;
        }

        return null;
    }

    if (isIntersection(obj)
        || isErsatzObject(obj)) {
        obj = obj.object;
    }

    if (!obj || !obj.visible) {
        return null;
    }

    return obj.userData[RAY_TARGET_KEY] as RayTarget<T>;
}

export function assureRayTarget<T = void>(obj: Objects): RayTarget<T> {
    if (!obj) {
        throw new Error("object is not defined");
    }

    return getRayTarget<T>(obj)
        || new RayTarget<T>(objectResolve(obj));
}