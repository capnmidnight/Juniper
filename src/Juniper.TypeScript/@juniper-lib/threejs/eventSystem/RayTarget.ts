import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { Mesh, Object3D } from "three";
import { ErsatzObject, objectIsFullyVisible, objectResolve, Objects } from "../objects";
import { Pointer3DEvents } from "./devices/Pointer3DEvent";

const RAY_TARGET_KEY = "Juniper:ThreeJS:EventSystem:RayTarget";

export class RayTarget<EventsT = void>
    extends TypedEventBase<EventsT & Pointer3DEvents>
    implements ErsatzObject {

    readonly meshes = new Array<Mesh>();

    private _disabled: boolean = false;
    private _clickable: boolean = false;
    private _draggable: boolean = false;
    private _navigable: boolean = false;

    constructor(public readonly object: Object3D) {
        super();
        this.object.userData[RAY_TARGET_KEY] = this;
    }

    addMesh(mesh: Mesh): this {
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

    get navigable() {
        return this._navigable;
    }

    set navigable(v) {
        this._navigable = v;
    }
}

export function isRayTarget<T = void>(obj: Objects): obj is RayTarget<T> {
    return obj instanceof RayTarget;
}

export function getRayTarget<T = void>(obj: Objects): RayTarget<T> {
    let target: RayTarget<T> = null;
    if (obj) {
        if (isRayTarget<T>(obj)) {
            target = obj;
        }
        else {
            obj = objectResolve(obj);
            if (obj) {
                target = obj.userData[RAY_TARGET_KEY] as RayTarget<T>;
            }
        }

        if (target && !objectIsFullyVisible(target)) {
            target = null;
        }
    }

    return target;
}